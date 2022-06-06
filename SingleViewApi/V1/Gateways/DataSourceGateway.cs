using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Factories;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.V1.Gateways
{
    public class DataSourceGateway : IDataSourceGateway
    {
        private readonly DatabaseContext _databaseContext;

        public DataSourceGateway(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public DBDataSource GetEntityById(int id)
        {
            var result = _databaseContext.DataSourceEntities.Find(id);

            return result?.ToDomain();
        }

        public List<DBDataSource> GetAll()
        {
            var results = _databaseContext.DataSourceEntities.ToList();
            return results.Map(x => x.ToDomain());
        }
    }
}
