namespace Application.Dtos.ReferalsUsers.Requests;

public record GetRewardRequest(Guid UserId, Guid InvitedUserId);