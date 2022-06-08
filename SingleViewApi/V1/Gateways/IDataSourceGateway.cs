using System.Collections.Generic;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways
{
    public interface IDataSourceGateway
    {
        DbDataSource GetEntityById(int id);

        List<DbDataSource> GetAll();
    }
}
