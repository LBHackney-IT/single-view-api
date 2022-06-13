using System;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways
{
    public interface ICustomerGateway
    {
        SavedCustomer Add(SavedCustomer customer);
        SavedCustomer Find(Guid id);

    }
}
