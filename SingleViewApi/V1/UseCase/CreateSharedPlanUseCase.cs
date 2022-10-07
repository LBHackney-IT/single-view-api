using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class CreateSharedPlanUseCase : ICreateSharedPlanUseCase
{
    private readonly string _sharedPlanBaseUrl;
    private readonly ISharedPlanGateway _sharedPlanGateway;

    public CreateSharedPlanUseCase(ISharedPlanGateway sharedPlanGateway, string sharedPlanBaseUrl)
    {
        _sharedPlanGateway = sharedPlanGateway;
        _sharedPlanBaseUrl = sharedPlanBaseUrl;
    }

    public async Task<CreateSharedPlanResponseObject> Execute(CreateSharedPlanRequest createSharedPlanRequest)
    {
        var response = await _sharedPlanGateway.CreateSharedPlan(createSharedPlanRequest);
        return new CreateSharedPlanResponseObject
        {
            Id = response.Id,
            FirstName = response.FirstName,
            LastName = response.LastName,
            SharedPlanUrl = $"{_sharedPlanBaseUrl}/plans/{response.Id}"
        };
    }
}
