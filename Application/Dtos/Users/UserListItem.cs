using Domain.Primitives.Enums;

namespace Application.Dtos.Users;

public record UserListItem(
    Guid Id,
    long IdTelegram,
    string Name,
    long Balance,
    RangTap RangTap);
