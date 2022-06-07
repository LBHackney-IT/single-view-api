using AutoFixture;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static DataSourceEntity CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<DbDataSource>();

            return CreateDatabaseEntityFrom(entity);
        }

        public static DataSourceEntity CreateDatabaseEntityFrom(DbDataSource entity)
        {
            return new DataSourceEntity()
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }
    }
}
