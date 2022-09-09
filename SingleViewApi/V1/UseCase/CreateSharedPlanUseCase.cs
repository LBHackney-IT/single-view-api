using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class CreateSharedPlanUseCase : ICreateSharedPlanUseCase
{
    private readonly ISharedPlanGateway _sharedPlanGateway;

    public CreateSharedPlanUseCase(ISharedPlanGateway sharedPlanGateway)
    {
        _sharedPlanGateway = sharedPlanGateway;
    }

    public async Task<CreateSharedPlanResponseObject> Execute(CreateSharedPlanRequest createSharedPlanRequest)
    {
        return await _sharedPlanGateway.CreateSharedPlan(createSharedPlanRequest);
    }
}
