using System;
using System.Collections.Generic;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways.Interfaces
{
    public interface ICustomerGateway
    {
        SavedCustomer Add(string firstName, string lastName, DateTime dateOfBirth, string niNumber);
        SavedCustomer Find(Guid id);
        List<SavedCustomer> Search(string firstName, string lastName);
        Guid? Delete(Guid id);
    }
}
