using Application.Interfaces.Services;
using Quartz;

namespace Infrastructure.Jobs;

public class UpdateEveryDayTasksJob(ITaskForRewardService taskForRewardService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await taskForRewardService.ReAcceptedAllEveryDayTasks(context.CancellationToken);
    }
}