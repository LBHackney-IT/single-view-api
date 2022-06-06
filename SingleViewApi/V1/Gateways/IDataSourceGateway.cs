using System.Collections.Generic;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways
{
    public interface IDataSourceGateway
    {
        DBDataSource GetEntityById(int id);

        List<DBDataSource> GetAll();
    }
}
