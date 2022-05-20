using System.Threading.Tasks;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetJigsawCustomerByIdUseCase
{
    Task<dynamic> Execute(string customerId, string redisId);
}
