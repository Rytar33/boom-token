using Application.Dtos;
using Application.Dtos.ReferalsUsers.Requests;
using Application.Dtos.ReferalsUsers.Responses;

namespace Application.Interfaces.Services;

public interface IReferalUsersService
{
    Task<GetYourReferalsResponse> GetYourReferals(Guid id, CancellationToken ct);
    
    Task<BaseResponse> GetReward(GetRewardRequest rewardRequest, CancellationToken ct);
}
