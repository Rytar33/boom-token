using Domain.Primitives.Enums;
using System.Net;

namespace Application.Dtos.Users.Responses;

public record DetailUserResponse : BaseResponse
{
    public DetailUserResponse(
        Guid id,
        long idTelegram,
        string name,
        long balance,
        RangTap rangTap,
        string referalLink,
        DateTime dateTimeRegistration,
        HttpStatusCode statusCode) : base(statusCode)
    {
        Id = id;
        IdTelegram = idTelegram;
        Name = name;
        Balance = balance;
        RangTap = rangTap;
        ReferalLink = referalLink;
        DateTimeRegistration = dateTimeRegistration;
    }

    public DetailUserResponse(
        HttpStatusCode statusCode,
        string errorMessage) : base(statusCode, errorMessage)
    {

    }
    public Guid? Id { get; set; }

    public long? IdTelegram { get; set; }

    public string? Name { get; set; }

    public long? Balance { get; set; }

    public RangTap? RangTap { get; set; }

    public string? ReferalLink { get; set; }

    public DateTime? DateTimeRegistration { get; set; }
}