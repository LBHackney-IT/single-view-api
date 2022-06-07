using AutoFixture;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.Tests.V1.Helper
{
    public static class DataSourceDbEntityHelper
    {
        public static DataSourceDbEntity CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<DbDataSource>();

            return CreateDatabaseEntityFrom(entity);
        }

        public static DataSourceDbEntity CreateDatabaseEntityFrom(DbDataSource entity)
        {
            return new DataSourceDbEntity()
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }
    }
}
