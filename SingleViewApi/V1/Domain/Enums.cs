using System.Text.Json.Serialization;

namespace SingleViewApi.V1.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TargetType
{
    Person,
    Asset,
    Tenure,
    Repair
}
