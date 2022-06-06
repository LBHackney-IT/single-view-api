using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Factories;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.V1.Gateways
{
    //TODO: Rename to match the data source that is being accessed in the gateway eg. MosaicGateway
    public class ExampleGateway : IExampleGateway
    {
        private readonly DatabaseContext _databaseContext;

        public ExampleGateway(DatabaseContext databaseContext)
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
