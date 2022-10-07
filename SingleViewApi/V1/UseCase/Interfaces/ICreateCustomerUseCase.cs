using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface ICreateCustomerUseCase
{
    SavedCustomer Execute(CreateCustomerRequest customerRequest);
}
