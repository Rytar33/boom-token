using Application.Dtos;
using Application.Dtos.TaskForRewards.Responses;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Validations;
using System.Net;
using Application.Dtos.TaskForRewards;
using Domain.Entities;
using Domain.Primitives.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public class TaskForRewardService(
    ITaskForRewardRepository taskForRewardRepository,
    ITaskForRewardAccessRepository taskForRewardAccessRepository,
    IUserRepository userRepository,
    IMapper mapper,
    IServiceProvider serviceProvider) : ITaskForRewardService
{
    
    public async Task<GetAllTaskResponse> GetAllTaskUser(Guid idUser, CancellationToken ct)
    {
        try
        {
            var tasksUser =
                await taskForRewardAccessRepository.GetAllAsync(t => 
                    t.IdUser == idUser, ct, t => t.TaskForReward!);
            var uniqueTasks = tasksUser
                .GroupBy(t => t.IdTaskForReward)
                .Select(g => g.OrderByDescending(t => t.DateTimeCompleted).First())
                .ToList();
            
            return new GetAllTaskResponse(
                HttpStatusCode.OK,
                uniqueTasks
                    .Where(t => t.TaskForReward!.TaskType == TaskType.Single)
                    .Select(t => new TaskListItem(
                        t.IdTaskForReward,
                        t.TaskForReward!.Name,
                        t.TaskForReward!.Reward,
                        t.DateTimeCompleted != null)),
                uniqueTasks
                    .Where(t => t.TaskForReward!.TaskType == TaskType.EveryDay)
                    .Select(t => new TaskListItem(
                        t.IdTaskForReward,
                        t.TaskForReward!.Name,
                        t.TaskForReward!.Reward,
                        t.DateTimeCompleted != null)));
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.ToString());
            return new GetAllTaskResponse(HttpStatusCode.InternalServerError, ErrorMessages.InternalServerError);
        }
    }

    public async Task<BaseResponse> UpdateCurrentValueTask(UpdateTaskRequest updateTaskRequest, CancellationToken ct)
    {
        try
        {
            var taskForReward = await taskForRewardAccessRepository.GetAsync(t =>
                    t.IdUser == updateTaskRequest.IdUser && t.IdTaskForReward == updateTaskRequest.IdTask, ct,
                t => t.TaskForReward!, t => t.User!);
            if (taskForReward == null)
                throw new ArgumentNullException(
                    "Данного задания и/или пользователя не было найденно, или оно не было принято");
            if (taskForReward.TaskForReward!.TargetType != TargetType.OpenLink)
                throw new ArgumentException(
                    "Невозможно обновить из-за попытки явно вызвать выполнение задание кроме ссылки");
            if (taskForReward.DateTimeCompleted != null)
                throw new ArgumentException("Данное задание уже было выполненно");

            taskForReward.Update(
                currentValue: taskForReward.CurrentValue + 1);
            if (taskForReward.TaskForReward.ProgressToCompletion <= taskForReward.CurrentValue)
            {
                taskForReward.Update(
                    currentValue: taskForReward.TaskForReward.ProgressToCompletion,
                    dateTimeCompleted: DateTime.Now);
                taskForReward.User!.Update(
                    balance: taskForReward.User.Balance + taskForReward.TaskForReward.Reward);
                await userRepository.UpdateAsync(taskForReward.User!, ct);
            }

            await taskForRewardAccessRepository.UpdateAsync(taskForReward, ct);
            await taskForRewardRepository.SaveChangesAsync(ct);

            return new BaseResponse(HttpStatusCode.NoContent);
        }
        catch (ArgumentNullException exc)
        {
            return new BaseResponse(HttpStatusCode.NotFound, exc.Message);
        }
        catch (ArgumentException exc)
        {
            return new BaseResponse(HttpStatusCode.BadRequest, exc.Message);
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.ToString());
            return new BaseResponse(HttpStatusCode.InternalServerError, ErrorMessages.InternalServerError);
        }
    }

    public async Task<BaseResponse> ReAcceptedAllEveryDayTasks(CancellationToken ct)
    {
        try
        {
            var tasksUsers = await taskForRewardAccessRepository
                .GetAllAsync(t => 
                        t.TaskForReward!.TaskType == TaskType.EveryDay, ct,
                    t => t.TaskForReward!);
            await taskForRewardAccessRepository.RemoveRange(tasksUsers);
            await taskForRewardAccessRepository.SaveChangesAsync(ct);

            var tasksEveryDay = 
                await taskForRewardRepository.GetAllAsync(t => t.TaskType == TaskType.EveryDay && t.IsActive, ct);
            tasksEveryDay = tasksEveryDay.ToList();
            foreach (var user in await userRepository.GetAllAsync(null, ct))
                await taskForRewardAccessRepository.AddRangeAsync(
                    tasksEveryDay.Select(t => 
                        new TaskForRewardAccess(user.Id, t.Id)), ct);
            await taskForRewardRepository.SaveChangesAsync(ct);
            
            return new BaseResponse(HttpStatusCode.NoContent);
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.ToString());
            return new BaseResponse(HttpStatusCode.InternalServerError, ErrorMessages.InternalServerError);
        }
    }
}
