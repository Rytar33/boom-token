using System.Net;

namespace Application.Dtos.Users.Responses;

public record ListUserResponse : BaseResponse
{
    public ListUserResponse(HttpStatusCode statusCode, IEnumerable<UserListItem> items) : base(statusCode)
    {
        Items = items;
    }
    
    public ListUserResponse(HttpStatusCode statusCode, string errorMessage) : base(statusCode, errorMessage)
    {
        
    }
    
    public IEnumerable<UserListItem> Items { get; set; } = [];
};
