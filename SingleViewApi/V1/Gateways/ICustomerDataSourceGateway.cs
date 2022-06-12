using System.Collections.Generic;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways
{
    public interface ICustomerDataSourceGateway
    {
        CustomerDataSource Add(CustomerDataSource customerDataSource);
    }
}
