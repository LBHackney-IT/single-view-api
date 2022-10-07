using System.Collections.Generic;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Boundary.Response;

public class SharedPlanResponseObject
{
    [JsonProperty("planIds")]
    public List<string> PlanIds { get; set; }
}
