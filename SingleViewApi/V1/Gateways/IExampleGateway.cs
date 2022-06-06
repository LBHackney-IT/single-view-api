using System.Collections.Generic;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways
{
    public interface IExampleGateway
    {
        DBDataSource GetEntityById(int id);

        List<DBDataSource> GetAll();
    }
}
