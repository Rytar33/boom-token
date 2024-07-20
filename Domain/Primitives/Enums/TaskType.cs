using System.Text.Json.Serialization;

namespace Domain.Primitives.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskType
{
    Single,
    EveryDay
}
