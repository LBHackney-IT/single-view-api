using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.V1.Factories
{
    public static class EntityFactory
    {
        public static DbDataSource ToDomain(this DataSourceEntity databaseEntity)
        {
            //TODO: Map the rest of the fields in the domain object.
            // More information on this can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Factory-object-mappings

            return new DbDataSource
            {
                Id = databaseEntity.Id,
                Name = databaseEntity.Name
            };
        }

        public static DataSourceEntity ToDatabase(this DbDataSource entity)
        {
            return new DataSourceEntity
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
    }
}
