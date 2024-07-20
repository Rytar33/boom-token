using System.Net;
using Application.Dtos;
using Application.Dtos.ReferalsUsers;
using Application.Dtos.ReferalsUsers.Requests;
using Application.Dtos.ReferalsUsers.Responses;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Application.Services;

public class ReferalUsersService(
    IReferalUsersRepository referalUsersRepository,
    IUserRepository userRepository,
    IServiceProvider serviceProvider) : IReferalUsersService
{
    private readonly ITelegramBotClient _telegramBot = serviceProvider.GetService<ITelegramBotClient>()!;
    
    public async Task<GetYourReferalsResponse> GetYourReferals(Guid id, CancellationToken ct)
    {
        try
        {
            var referalsUsers = await referalUsersRepository.GetAllAsync(
                ru => ru.IdUser == id, ct, ru => ru.User!, ru => ru.UserInvited!);
            var referalItems = new List<ReferalUsersListItem>();
            foreach (var referalUsers in referalsUsers)
            {
                var tgUser = await _telegramBot.GetChatAsync(referalUsers.UserInvited!.TelegramId, ct);
                referalItems.Add(
                    new ReferalUsersListItem(
                        referalUsers.IdUserInvited,
                        tgUser.FirstName,
                        1000,
                        CalculationReward(
                            referalUsers.UserInvited.CountClick,
                            referalUsers.CountTakeFromClick,
                            referalUsers.UserInvited.RewardForClick)));
            }
            return new GetYourReferalsResponse(HttpStatusCode.OK, referalItems);
        }
        catch (Exception exc)
        {
            return new GetYourReferalsResponse(HttpStatusCode.InternalServerError, exc.ToString());
        }
    }

    public async Task<BaseResponse> GetReward(GetRewardRequest getRewardRequest, CancellationToken ct)
    {
        try
        {
            var referalUsers = await referalUsersRepository.GetAsync(ru =>
                ru.IdUserInvited == getRewardRequest.InvitedUserId 
                && ru.IdUser == getRewardRequest.UserId, ct,
                ru => ru.User!, ru => ru.UserInvited!);
            if (referalUsers == null)
                throw new ArgumentNullException("Данн(ого/ых) пользовател(я/ей) не было найденно");
            referalUsers.User!.Update(
                balance: referalUsers.User!.Balance + CalculationReward(
                    referalUsers.UserInvited!.CountClick,
                    referalUsers.CountTakeFromClick,
                    referalUsers.UserInvited.RewardForClick));
            await userRepository.UpdateAsync(referalUsers.User!, ct);
            referalUsers.Update(countTakeFromClick:
                referalUsers.UserInvited!.CountClick);
            await referalUsersRepository.UpdateAsync(referalUsers, ct);
            await userRepository.SaveChangesAsync(ct);
            return new BaseResponse(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return new BaseResponse(HttpStatusCode.InternalServerError, ex.ToString());
        }
    }
    
    private static int CalculationReward(long countClick, long countTakeFromClick, int rewardForClick)
        => (int)Math.Round(((countClick - countTakeFromClick) * rewardForClick) * 0.05, 0);
}
