using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetCouncilTaxAccountByIdUseCase
{
    Task<CustomerResponseObject> Execute(string id, string userToken);
}
