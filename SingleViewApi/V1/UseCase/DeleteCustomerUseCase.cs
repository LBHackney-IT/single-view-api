using System;
using Hackney.Core.Logging;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class DeleteCustomerUseCase : IDeleteCustomerUseCase
{
    private readonly ICustomerGateway _customerGateway;

    public DeleteCustomerUseCase(ICustomerGateway customerGateway)
    {
        _customerGateway = customerGateway;
    }

    [LogCall]
    public Boolean Execute(Guid customerId)
    {
        var result = _customerGateway.Delete(customerId);

        return result != null;
    }
}
