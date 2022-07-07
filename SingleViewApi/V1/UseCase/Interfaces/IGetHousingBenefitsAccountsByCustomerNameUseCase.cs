using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetHousingBenefitsAccountsByCustomerNameUseCase
{
    Task<SearchResponseObject> Execute(string firstName, string lastName, string userToken);
}
