using Application.Dtos;
using Application.Dtos.TaskForRewards;
using Application.Dtos.TaskForRewards.Responses;

namespace Application.Interfaces.Services;

public interface ITaskForRewardService
{
    Task<GetAllTaskResponse> GetAllTaskUser(Guid idUser, CancellationToken ct);
    
    Task<BaseResponse> UpdateCurrentValueTask(UpdateTaskRequest updateTaskRequest, CancellationToken ct);

    Task<BaseResponse> ReAcceptedAllEveryDayTasks(CancellationToken ct);
}
