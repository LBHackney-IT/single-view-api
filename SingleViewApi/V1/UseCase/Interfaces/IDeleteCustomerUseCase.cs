using System;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IDeleteCustomerUseCase
{
    bool Execute(Guid customerId);
}
