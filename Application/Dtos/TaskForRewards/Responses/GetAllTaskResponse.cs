using System.Net;

namespace Application.Dtos.TaskForRewards.Responses;

public record GetAllTaskResponse : BaseResponse
{
    public GetAllTaskResponse(
        HttpStatusCode httpStatusCode,
        IEnumerable<TaskListItem> itemsSingleTask,
        IEnumerable<TaskListItem> itemsEveryDayTask)
        : base(httpStatusCode)
    {
        ItemsSingleTask = itemsSingleTask;
        ItemsEveryDayTask = itemsEveryDayTask;
    }

    public GetAllTaskResponse(HttpStatusCode statusCode, string errorMessage)
        : base(statusCode, errorMessage)
    { 
    
    }

    public IEnumerable<TaskListItem> ItemsSingleTask { get; set; } = [];

    public IEnumerable<TaskListItem> ItemsEveryDayTask { get; set; } = [];
}
