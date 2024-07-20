using System.Net;
using Application.Dtos;
using Application.Dtos.Improvements;
using Application.Dtos.Improvements.Requests;
using Application.Dtos.Improvements.Responses;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Primitives.Enums;

namespace Application.Services;

public class ImprovementService(
    IImprovementRepository improvementRepository,
    IImprovementAccessRepository improvementAccessRepository,
    IUserRepository userRepository,
    IMapper mapper) : IImprovementService
{
    
    public async Task<BaseResponse> BuyImprovement(BuyImprovementRequest buyImprovementRequest, CancellationToken ct)
    {
        try
        {
            var improvementToBuy = await improvementRepository.GetAsync(
                i => i.Id == buyImprovementRequest.IdImprovement, ct);

            if (improvementToBuy == null)
                throw new ArgumentNullException("Улучшение не найдено");

            var userImprovements = await improvementAccessRepository.GetAllAsync(
                ia => ia.IdUser == buyImprovementRequest.IdUser,
                ct,
                ia => ia.Improvement!);

            if (userImprovements.Any(ia => ia.Improvement!.Id == buyImprovementRequest.IdImprovement))
                throw new ArgumentException("Вы не можете купить улучшение, так как оно уже было куплено вами");

            var previousLevelImprovement = userImprovements.FirstOrDefault(ia =>
                ia.Improvement!.ImprovementType == improvementToBuy.ImprovementType &&
                ia.Improvement.Level == improvementToBuy.Level - 1);

            if (improvementToBuy.Level > 1 && previousLevelImprovement == null)
                throw new ArgumentException(
                    "Вы не можете купить улучшение, так как не купили улучшение предыдущего уровня");

            var getUser = await userRepository.GetAsync(u => u.Id == buyImprovementRequest.IdUser, ct);

            if (getUser == null)
                throw new ArgumentNullException("Данного пользователя не было найдено");

            if (getUser.Balance < improvementToBuy.Cost)
                throw new ArgumentException("Ваш баланс недостаточен для покупки данного улучшения");

            await improvementAccessRepository.AddAsync(
                new ImprovementAccess(buyImprovementRequest.IdUser, buyImprovementRequest.IdImprovement), ct);
            
            getUser.Update(
                balance: getUser.Balance - improvementToBuy.Cost);

            switch (improvementToBuy.ImprovementType)
            {
                case ImprovementType.EnergyLimit:
                    getUser.Update(
                        limitEnergy: getUser.LimitEnergy + improvementToBuy.ValueIncreased);
                    break;
                case ImprovementType.SpeedEnergyRecovery:
                    getUser.Update(
                        energyRecoveryInSecond: getUser.EnergyRecoveryInSecond + improvementToBuy.ValueIncreased);
                    break;
                case ImprovementType.ProfitPerTap:
                    getUser.Update(
                        rewardForClick: getUser.RewardForClick + improvementToBuy.ValueIncreased);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            await userRepository.UpdateAsync(getUser, ct);
            
            await improvementRepository.SaveChangesAsync(ct);

            return new BaseResponse(HttpStatusCode.NoContent);
        }
        catch (ArgumentNullException ex)
        {
            return new BaseResponse(HttpStatusCode.NotFound, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return new BaseResponse(HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception exc)
        {
            return new BaseResponse(HttpStatusCode.InternalServerError, exc.ToString());
        }
    }

    public async Task<GetImprovementListResponse> GetNextImprovement(Guid idUser, CancellationToken ct)
    {
        try
        {
            var getAllImprovementUser = await improvementAccessRepository.GetAllAsync(
            i => i.IdUser == idUser,
            ct,
            i => i.Improvement!);

            var getAllImprovement = await improvementRepository.GetAllAsync(null, ct);

            var userImprovements = getAllImprovementUser
                .Select(ia => ia.Improvement)
                .Where(i => i != null)
                .ToList();

            var groupedImprovements = getAllImprovement
                .GroupBy(i => i.ImprovementType)
                .Select(g =>
                {
                    var userMaxLevelImprovement = userImprovements
                        .Where(i => i.ImprovementType == g.Key)
                        .OrderBy(i => i.Level)
                        .LastOrDefault();

                    if (userMaxLevelImprovement != null)
                    {
                        var nextLevel = userMaxLevelImprovement.Level + 1;
                        var nextLevelImprovement = g.FirstOrDefault(i => i.Level == nextLevel);

                        return nextLevelImprovement != null
                            ? new ImprovementListItem(
                                nextLevelImprovement.Id,
                                nextLevelImprovement.ImprovementType,
                                nextLevelImprovement.Name,
                                nextLevelImprovement.Cost,
                                false)
                            : new ImprovementListItem(
                                userMaxLevelImprovement.Id,
                                userMaxLevelImprovement.ImprovementType,
                                userMaxLevelImprovement.Name,
                                userMaxLevelImprovement.Cost,
                                true);
                    }
                    else
                    {
                        var minLevelImprovement = g.OrderBy(i => i.Level).First();
                        return new ImprovementListItem(
                            minLevelImprovement.Id,
                            minLevelImprovement.ImprovementType,
                            minLevelImprovement.Name,
                            minLevelImprovement.Cost,
                            false);
                    }
                })
                .ToList();

            return new GetImprovementListResponse(HttpStatusCode.OK, groupedImprovements);
        }
        catch (Exception exc)
        {
            return new GetImprovementListResponse(HttpStatusCode.InternalServerError, exc.ToString());
        }
    }
}
