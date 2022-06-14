using System;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways
{
    public interface ICustomerDataSourceGateway
    {
        CustomerDataSource Add(Guid customerId, int dataSourceId, string sourceId);
    }
}
