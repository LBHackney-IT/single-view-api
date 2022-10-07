using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.V1.Factories
{
    public static class DataSourceFactory
    {
        public static DataSource ToDomain(this DataSourceDbEntity dataSourceDbEntity)
        {
            // TODO: Map the rest of the fields in the domain object.
            // More information on this can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Factory-object-mappings

            return new DataSource
            {
                Id = dataSourceDbEntity.Id,
                Name = dataSourceDbEntity.Name
            };
        }

        public static DataSourceDbEntity ToDatabase(this DataSource entity)
        {
            return new DataSourceDbEntity
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
    }
}
