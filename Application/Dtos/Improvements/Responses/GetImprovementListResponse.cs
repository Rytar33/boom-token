using System.Net;

namespace Application.Dtos.Improvements.Responses;

public record GetImprovementListResponse : BaseResponse
{
    public GetImprovementListResponse(
        HttpStatusCode statusCode,
        string errorMessage)
        : base(statusCode, errorMessage)
    {

    }

    public GetImprovementListResponse(
        HttpStatusCode statusCode,
        IEnumerable<ImprovementListItem> improvementItems)
        : base(statusCode)
    {
        ImprovementItems = improvementItems;
    }

    public IEnumerable<ImprovementListItem> ImprovementItems { get; set; } = [];
}