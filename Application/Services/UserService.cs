using Application.Dtos;
using Application.Dtos.Users;
using Application.Dtos.Users.Responses;
using Application.Interfaces.Options;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Primitives.Enums;
using Domain.Validations;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Application.Dtos.Enums;
using Application.Dtos.Enums.Response;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Application.Services;

public class UserService(
    IUserRepository userRepository,
    IReferalUsersRepository referalUsersRepository,
    ITaskForRewardAccessRepository taskForRewardAccessRepository,
    ITaskForRewardRepository taskForRewardRepository,
    IMapper mapper,
    IJwtOption jwtOptions,
    IServiceProvider serviceProvide) : IUserService
{
    private readonly ITelegramBotClient _telegramBotClient = serviceProvide.GetService<ITelegramBotClient>()!;
    
    public async Task<BaseResponse> RegistrationUser(CreateUserRequest createUserRequest, CancellationToken ct)
    {
        try
        {
            var getChat = await _telegramBotClient.GetChatAsync(createUserRequest.IdTelegram, ct);
            var user = mapper.Map<User>(createUserRequest);
            await userRepository.AddAsync(user, ct);
            await userRepository.SaveChangesAsync(ct);
            var tasks = await taskForRewardRepository.GetAllAsync(t => t.IsActive, ct);
            foreach (var taskForReward in tasks)
                await taskForRewardAccessRepository.AddAsync(
                    new TaskForRewardAccess(
                        user.Id,
                        taskForReward.Id), ct);
            await taskForRewardAccessRepository.SaveChangesAsync(ct);
            if (!string.IsNullOrWhiteSpace(createUserRequest.ReferalLink))
            {
                var getUserByReferal = await userRepository.GetAsync(u => u.ReferalLink == createUserRequest.ReferalLink, ct);
                if (getUserByReferal != null)
                {
                    await referalUsersRepository.AddAsync(new ReferalUsers(getUserByReferal.Id, user.Id), ct);
                    getUserByReferal.Update(balance: getUserByReferal.Balance + 1000);
                    await userRepository.UpdateAsync(getUserByReferal, ct);
                    var tasksUser = await taskForRewardAccessRepository.GetAllAsync(
                        t => t.IdUser == getUserByReferal.Id
                             && t.DateTimeCompleted == null
                             && t.TaskForReward!.TargetType == TargetType.ReferalInvite,
                        ct,
                        t => t.TaskForReward!);
                    foreach (var taskUser in tasksUser)
                    {
                        taskUser.Update(currentValue: taskUser.CurrentValue + 1);
                        if (taskUser.CurrentValue >= taskUser.TaskForReward!.ProgressToCompletion)
                        {
                            taskUser.Update(
                                dateTimeCompleted: DateTime.Now);
                            getUserByReferal.Update(balance: getUserByReferal.Balance + taskUser.TaskForReward.Reward);
                            await userRepository.UpdateAsync(getUserByReferal, ct);
                        }
                        await taskForRewardAccessRepository.UpdateAsync(taskUser, ct);
                    }
                    await userRepository.SaveChangesAsync(ct);
                }
            }
            return new BaseResponse(HttpStatusCode.Created);
        }
        catch (ArgumentException exception)
        {
            return new BaseResponse(HttpStatusCode.BadRequest, exception.Message);
        }
        catch (ValidationException exception)
        {
            return new BaseResponse(HttpStatusCode.BadRequest, exception.Errors);
        }
        catch (Exception exc)
        {
            return new BaseResponse(HttpStatusCode.InternalServerError, exc.ToString());
        }
    }

    public async Task<AutentiticationUserResponse> LogIn(long id, CancellationToken ct)
    {
        try
        {
            var user = await userRepository.GetAsync(u => u.TelegramId == id, ct);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var jwt = new JwtSecurityToken(
                issuer: jwtOptions.Issure,
                audience: jwtOptions.Audience,
                notBefore: DateTime.Now,
                claims: [new Claim(nameof(User.Id), user.Id.ToString())],
                expires: DateTime.Now.AddHours(Convert.ToInt32(jwtOptions.Expires)),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    SecurityAlgorithms.HmacSha256));

            return new AutentiticationUserResponse(new JwtSecurityTokenHandler().WriteToken(jwt), HttpStatusCode.OK);
        }
        catch (ArgumentNullException)
        {
            return new AutentiticationUserResponse(null, HttpStatusCode.NotFound, ErrorMessages.NotFoundError);
        }
        catch (Exception exc)
        {
            return new AutentiticationUserResponse(null, HttpStatusCode.InternalServerError, exc.ToString());
        }
    }

    public async Task<DetailUserResponse> GetDetailUser(Guid idUser, CancellationToken ct)
    {
        try
        {
            var user = await userRepository.GetAsync(u => u.Id == idUser, ct);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var detailUser = mapper.Map<DetailUserResponse>(user);
            detailUser.StatusCode = HttpStatusCode.OK;
            return detailUser;
        }
        catch(ArgumentNullException)
        {
            return new DetailUserResponse(
                HttpStatusCode.NotFound,
                string.Format(ErrorMessages.NotFoundError, nameof(User)));
        }
        catch (Exception exc)
        {
            return new DetailUserResponse(HttpStatusCode.InternalServerError, exc.ToString());
        }
    }

    public async Task<ListUserResponse> GetListUsers(CancellationToken ct)
    {
        try
        {
            return mapper.Map<ListUserResponse>(await userRepository.GetAllAsync(null, ct));
        }
        catch (Exception exc)
        {
            return new ListUserResponse(HttpStatusCode.InternalServerError, exc.ToString());
        }

    }

    public async Task<GetBalanceResponse> GetBalanceUser(Guid idUser, CancellationToken ct)
    {
        try
        {
            var getUser = await userRepository.GetAsync(u => u.Id == idUser, ct);
            if (getUser == null)
                throw new ArgumentNullException("Данного пользователя не было найдено");
            return new GetBalanceResponse(HttpStatusCode.OK, getUser.Id, getUser.Balance);
        }
        catch (ArgumentNullException exc)
        {
            return new GetBalanceResponse(HttpStatusCode.NotFound, exc.Message);
        }
        catch (Exception exc)
        {
            return new GetBalanceResponse(HttpStatusCode.InternalServerError, exc.ToString());
        }
    }

    public async Task<GetReferalLinkUserResponse> GetReferalLink(Guid idUser, CancellationToken ct)
    {
        try
        {
            var getUser = await userRepository.GetAsync(u => u.Id == idUser, ct);
            if (getUser == null)
                throw new ArgumentNullException("Данного пользователя не было найдено");
            return new GetReferalLinkUserResponse(HttpStatusCode.OK, getUser.Id, getUser.ReferalLink);
        }
        catch (ArgumentNullException exc)
        {
            return new GetReferalLinkUserResponse(HttpStatusCode.NotFound, exc.Message);
        }
        catch (Exception exc)
        {
            return new GetReferalLinkUserResponse(HttpStatusCode.InternalServerError, exc.ToString());
        }
    }
    
    public async Task<GetRangeRankResponse> GetRangeRang(Guid idUser, CancellationToken ct)
    {
        try
        {
            var getUser = await userRepository.GetAsync(u => u.Id == idUser, ct);
            if (getUser == null)
                throw new ArgumentNullException("Данного пользователя не было найдено");
            var valuesRangTap = Enum.GetValues(typeof(RangTap)).Cast<int>().ToList();
            var currentIndex = valuesRangTap.IndexOf((int)getUser.RangTap);
            EnumItem? endRange = null;
            
            if (currentIndex < valuesRangTap.Count - 1)
            {
                var rangTapNext = (RangTap)valuesRangTap[currentIndex + 1];
                endRange = new EnumItem(rangTapNext.ToString(), (int)rangTapNext);
            }
            
            return new GetRangeRankResponse(HttpStatusCode.OK,
                new EnumItem(getUser.RangTap.ToString(), (int)getUser.RangTap), endRange);
        }
        catch (ArgumentNullException exc)
        {
            return new GetRangeRankResponse(HttpStatusCode.NotFound, exc.Message);
        }
        catch (Exception exc)
        {
            return new GetRangeRankResponse(HttpStatusCode.InternalServerError, exc.ToString());
        }
    }
    
    public async Task<BaseResponse> Click(Guid idUser, CancellationToken ct)
    {
        try
        {
            var user = await userRepository.GetAsync(u => u.Id == idUser, ct);
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            user.Update(
                energy: user.Energy - 1,
                balance: user.Balance + user.RewardForClick,
                countClick: user.CountClick + 1);
            await userRepository.UpdateAsync(user, ct);
            await userRepository.SaveChangesAsync(ct);
            
            var tasksUser = await taskForRewardAccessRepository.GetAllAsync(
                t => t.IdUser == idUser
                && t.DateTimeCompleted == null
                && t.TaskForReward!.TargetType == TargetType.Tap,
                ct,
                t => t.TaskForReward!);

            foreach (var taskUser in tasksUser)
            {
                taskUser.Update(
                        currentValue: taskUser.CurrentValue + 1);
                if (taskUser.CurrentValue >= taskUser.TaskForReward!.ProgressToCompletion)
                {
                    taskUser.Update(
                        dateTimeCompleted: DateTime.Now);
                    user.Update(balance: user.Balance + taskUser.TaskForReward.Reward);
                    await userRepository.UpdateAsync(user, ct);
                }
                await taskForRewardAccessRepository.UpdateAsync(taskUser, ct);
                await taskForRewardAccessRepository.SaveChangesAsync(ct);
            }
            return new BaseResponse(HttpStatusCode.NoContent);
        }
        catch (ArgumentNullException)
        {
            return new BaseResponse(HttpStatusCode.NotFound, string.Format(ErrorMessages.NotFoundError, "пользователя"));
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc);
            return new BaseResponse(HttpStatusCode.InternalServerError, exc.ToString());
        }
    }

    public async Task<BaseResponse> RecoveryEnergyAllUsers(CancellationToken ct)
    {
        try
        {
            var users = await userRepository.GetAllAsync(null, ct);
            foreach (var user in users)
            {
                if (user.Energy >= user.LimitEnergy)
                    continue;
                user.Update(energy: user.Energy + user.EnergyRecoveryInSecond);
                await userRepository.UpdateAsync(user, ct);
            }
            await userRepository.SaveChangesAsync(ct);
            return new BaseResponse(HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            return new BaseResponse(HttpStatusCode.InternalServerError, ex.ToString());
        }
    }
}
