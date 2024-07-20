using System.Net;

namespace Application.Dtos.Users.Responses;

public record AutentiticationUserResponse(
    string? Token,
    HttpStatusCode StatusCode,
    string? ErrorMessage = null)
    : BaseResponse(StatusCode, ErrorMessage);
