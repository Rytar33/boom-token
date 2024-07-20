using System.Net;

namespace Application.Dtos.Users.Responses;

public record GetBalanceResponse : BaseResponse
{
    public GetBalanceResponse(
        HttpStatusCode statusCode,
        string errorMessage) : base(statusCode, errorMessage)
    {

    }

    public GetBalanceResponse(HttpStatusCode statusCode, Guid idUser, long balance) : base(statusCode)
    {
        IdUser = idUser;
        Balance = balance;
    }
    
    public Guid IdUser { get; set; }
    
    public long Balance { get; set; }
}