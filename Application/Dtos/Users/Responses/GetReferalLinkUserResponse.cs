using System.Net;

namespace Application.Dtos.Users.Responses;

public record GetReferalLinkUserResponse : BaseResponse
{
    public GetReferalLinkUserResponse(HttpStatusCode statusCode, string errorMessage) : base(statusCode, errorMessage)
    {
        
    }

    public GetReferalLinkUserResponse(HttpStatusCode statusCode, Guid idUser, string referalLink) : base(statusCode)
    {
        IdUser = idUser;
        ReferalLink = referalLink;
    }
    
    public Guid IdUser { get; set; }
    
    public string ReferalLink { get; set; }
}