using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Factories;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.V1.Gateways
{
    public class DataSourceGateway : IDataSourceGateway
    {
        private readonly SingleViewContext _singleViewContext;

        public DataSourceGateway(SingleViewContext singleViewContext)
        {
            _singleViewContext = singleViewContext;
        }

        public DataSource GetEntityById(int id)
        {
            var result = _singleViewContext.DataSources.Find(id);

            return result?.ToDomain();
        }

        public List<DataSource> GetAll()
        {
            var results = _singleViewContext.DataSources.ToList();
            return results.Map(x => x.ToDomain());
        }
    }
}
