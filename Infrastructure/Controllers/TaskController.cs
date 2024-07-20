using Application.Dtos.TaskForRewards;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Controllers;

[Route("api")]
[ApiController]
public class TaskController(
    ITaskForRewardService taskForRewardService) : Controller
{
    [HttpPatch("[controller]/UpdateCurrentValue")]
    [Authorize]
    public async Task<IActionResult> UpdateTask(UpdateTaskRequest updateTaskRequest, CancellationToken ct)
    {
        var result = await taskForRewardService.UpdateCurrentValueTask(updateTaskRequest, ct);
        return StatusCode((int)result.StatusCode, result);
    }
    
    [HttpGet("[controller]s/AllTasksUser")]
    [Authorize]
    public async Task<IActionResult> GetAllTasksUser(Guid idUser, CancellationToken ct)
    {
        var result = await taskForRewardService.GetAllTaskUser(idUser, ct);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("[controller]")]
    public async Task<IActionResult> ReAcceptedTasks(CancellationToken ct)
    {
        var result = await taskForRewardService.ReAcceptedAllEveryDayTasks( ct);
        return StatusCode((int)result.StatusCode, result);
    }
}
