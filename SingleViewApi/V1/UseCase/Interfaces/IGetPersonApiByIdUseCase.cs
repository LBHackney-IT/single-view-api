using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetPersonApiByIdUseCase
{
    Task<CustomerResponseObject> Execute(string personId, string userToken);
}
