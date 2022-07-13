using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetHousingBenefitsAccountByAccountRefUseCase
{
    Task<CustomerResponseObject> Execute(string accountRef, string userToken);
}
