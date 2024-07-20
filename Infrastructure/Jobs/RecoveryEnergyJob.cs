using Application.Interfaces.Services;
using Quartz;

namespace Infrastructure.Jobs;

public class RecoveryEnergyJob(
    IUserService userService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await userService.RecoveryEnergyAllUsers(context.CancellationToken);
    }
}