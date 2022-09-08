using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Request;

public class GetSharedPlanRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<string> SystemIds { get; set; }
}
