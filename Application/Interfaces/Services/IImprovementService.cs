using Application.Dtos;
using Application.Dtos.Improvements.Requests;
using Application.Dtos.Improvements.Responses;

namespace Application.Interfaces.Services;

public interface IImprovementService
{
    Task<BaseResponse> BuyImprovement(BuyImprovementRequest buyImprovementRequest, CancellationToken ct);

    Task<GetImprovementListResponse> GetNextImprovement(Guid idUser, CancellationToken ct);
}
