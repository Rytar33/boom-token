using Application.Dtos;
using Application.Dtos.Enums.Response;
using Application.Dtos.Users;
using Application.Dtos.Users.Responses;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<BaseResponse> RegistrationUser(CreateUserRequest createUserRequest, CancellationToken ct);

    Task<AutentiticationUserResponse> LogIn(long id, CancellationToken ct);

    Task<DetailUserResponse> GetDetailUser(Guid idUser, CancellationToken ct);

    Task<ListUserResponse> GetListUsers(CancellationToken ct);

    Task<GetBalanceResponse> GetBalanceUser(Guid idUser, CancellationToken ct);

    Task<GetReferalLinkUserResponse> GetReferalLink(Guid idUser, CancellationToken ct);
    
    Task<GetRangeRankResponse> GetRangeRang(Guid idUser, CancellationToken ct);
    
    Task<BaseResponse> Click(Guid idUser, CancellationToken ct);
    
    Task<BaseResponse> RecoveryEnergyAllUsers(CancellationToken ct);
}
