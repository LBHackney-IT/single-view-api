using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Request;

public class CreateSharedPlanRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<string> SystemIds { get; set; }
    public List<string> Numbers { get; set; }
    public List<string> Emails { get; set; }
    public bool HasPhp { get; set; }
}
