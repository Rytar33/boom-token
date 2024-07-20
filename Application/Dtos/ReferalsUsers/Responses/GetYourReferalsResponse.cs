using System.Net;

namespace Application.Dtos.ReferalsUsers.Responses;

public record GetYourReferalsResponse : BaseResponse
{
    public GetYourReferalsResponse(HttpStatusCode statusCode, string errorMessage) : base(statusCode, errorMessage)
    {

    }

    public GetYourReferalsResponse(HttpStatusCode statusCode, IEnumerable<ReferalUsersListItem> referalItems) : base(statusCode)
    {
        ReferalItems = referalItems;
    }

    public IEnumerable<ReferalUsersListItem> ReferalItems { get; set; } = [];
}
