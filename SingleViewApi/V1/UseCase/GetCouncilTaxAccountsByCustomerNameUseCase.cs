using Hackney.Core.Logging;
using ServiceStack.MiniProfiler.Data;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetCouncilTaxAccountsByCustomerNameUseCase : IGetCouncilTaxAccountsByCustomerNameUseCase
{
    private readonly IAcademyGateway _academyGateway;

    public GetCouncilTaxAccountsByCustomerNameUseCase(IAcademyGateway academyGateway)
    {
        _academyGateway = academyGateway;
    }

    [LogCall]
    public dynamic Execute(string firstName, string lastName, string userToken)
    {
        return "placeholder";
    }
}
