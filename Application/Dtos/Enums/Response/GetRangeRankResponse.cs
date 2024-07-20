using System.Net;

namespace Application.Dtos.Enums.Response;

public record GetRangeRankResponse : BaseResponse
{
    public GetRangeRankResponse(HttpStatusCode statusCode, string errorMessage) 
        : base(statusCode, errorMessage)
    {
        
    }

    public GetRangeRankResponse(
        HttpStatusCode statusCode,
        EnumItem beginRange,
        EnumItem? endRange) : base(statusCode)
    {
        BeginRange = beginRange;
        EndRange = endRange;
    }
    
    public EnumItem BeginRange { get; set; }
    
    public EnumItem? EndRange { get; set; }
}