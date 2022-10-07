using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.V1.Factories;

public static class CustomerDataSourceFactory
{
    public static CustomerDataSource ToDomain(this CustomerDataSourceDbEntity customerDataSourceDbEntity)
    {
        // TODO: Map the rest of the fields in the domain object.
        // More information on this can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Factory-object-mappings

        return new CustomerDataSource
        {
            Id = customerDataSourceDbEntity.Id,
            CustomerId = customerDataSourceDbEntity.CustomerDbEntityId,
            DataSourceId = customerDataSourceDbEntity.DataSourceId,
            SourceId = customerDataSourceDbEntity.SourceId
        };
    }

    public static CustomerDataSourceDbEntity ToDatabase(this CustomerDataSource entity)
    {
        return new CustomerDataSourceDbEntity
        {
            CustomerDbEntityId = entity.CustomerId, DataSourceId = entity.DataSourceId, SourceId = entity.SourceId
        };
    }
}
