using System.Threading.Tasks;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetJigsawCasesByCustomerIdUseCase
{
    Task<CasesResponseObject> Execute(string customerId, string redisId, string hackneyToken);
}
