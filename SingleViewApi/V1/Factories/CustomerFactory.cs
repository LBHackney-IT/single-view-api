using System.Collections.Generic;
using System.Linq;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.V1.Factories
{
    public static class CustomerFactory
    {
        public static SavedCustomer ToDomain(this CustomerDbEntity customerDbEntity)
        {
            // TODO: Map the rest of the fields in the domain object.
            // More information on this can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Factory-object-mappings

            return new SavedCustomer
            {
                Id = customerDbEntity.Id,
                FirstName = customerDbEntity.FirstName,
                LastName = customerDbEntity.LastName,
                DateOfBirth = customerDbEntity.DateOfBirth,
                NiNumber = customerDbEntity.NiNumber,
                CreatedAt = customerDbEntity.UpdatedAt,
                UpdatedAt = customerDbEntity.UpdatedAt,
                DataSources = customerDbEntity.DataSources.ToDomain()
            };
        }

        private static List<CustomerDataSource> ToDomain(this List<CustomerDataSourceDbEntity> dataSources)
        {
            return dataSources?.Select(hrItem => hrItem.ToDomain()).ToList();
        }

        public static CustomerDbEntity ToDatabase(this SavedCustomer entity)
        {
            return new CustomerDbEntity
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                DateOfBirth = entity.DateOfBirth,
                NiNumber = entity.NiNumber
                // CreatedAt = entity.UpdatedAt,
                // UpdatedAt = entity.UpdatedAt,
            };
        }
    }
}
