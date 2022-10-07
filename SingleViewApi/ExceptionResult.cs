using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SingleViewApi;

public class ExceptionResult
{
    public const int DefaultStatusCode = 500;

    private static readonly JsonSerializerSettings _settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public ExceptionResult(string message, string traceId, string correlationId, int statusCode = DefaultStatusCode)
    {
        Message = message;
        TraceId = traceId;
        CorrelationId = correlationId;
        StatusCode = statusCode;
    }

    public string Message { get; }
    public string TraceId { get; }
    public string CorrelationId { get; }
    public int StatusCode { get; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, _settings);
    }
}
