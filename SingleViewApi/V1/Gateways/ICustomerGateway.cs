using System;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways
{
    public interface ICustomerGateway
    {
        SavedCustomer Add(string firstName, string lastName, DateTime dateOfBirth, string niNumber);
        SavedCustomer Find(Guid id);

    }
}
