using System.Text.Json.Serialization;

namespace Domain.Primitives.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RangTap
{
    White = 0,
    Bronza = 10000,
    Silver = 20000,
    Gold = 35000,
    Platinum = 50000,
    Briliant = 100000
}
