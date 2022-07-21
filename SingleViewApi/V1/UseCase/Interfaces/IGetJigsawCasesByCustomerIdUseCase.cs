using System.Threading.Tasks;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetJigsawCasesByCustomerIdUseCase
{
    Task<CasesResponseObject> Execute(string customerId, string redisId, string hackneyToken);
}
