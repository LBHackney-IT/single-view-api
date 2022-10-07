using System;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IDeleteCustomerUseCase
{
    Boolean Execute(Guid customerId);
}
