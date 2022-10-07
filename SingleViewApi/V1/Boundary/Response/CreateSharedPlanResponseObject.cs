namespace SingleViewApi.V1.Boundary.Response;

public class CreateSharedPlanResponseObject
{
    public string Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string SharedPlanUrl { get; set; }
}
