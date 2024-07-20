using FluentValidation.Results;
using System.Net;

namespace Application.Dtos;

public record BaseResponse 
{
    public BaseResponse(
        HttpStatusCode statusCode,
        string? errorMessage = null)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }

    public BaseResponse(
        HttpStatusCode statusCode,
        IEnumerable<ValidationFailure> errorsValidation)
    {
        StatusCode = statusCode;
        ErrorsValidation = errorsValidation;
    }

    public HttpStatusCode StatusCode { get; set; }

    public string? ErrorMessage { get; set; }

    public IEnumerable<ValidationFailure>? ErrorsValidation { get; set; }
}