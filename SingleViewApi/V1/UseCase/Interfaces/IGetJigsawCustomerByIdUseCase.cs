using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetJigsawCustomerByIdUseCase
{
    Task<CustomerResponseObject> Execute(string customerId, string redisId);
}
