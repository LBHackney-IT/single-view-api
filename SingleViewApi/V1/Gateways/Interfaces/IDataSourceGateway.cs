using System.Collections.Generic;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways.Interfaces;

public interface IDataSourceGateway
{
    DataSource GetEntityById(int id);

    List<DataSource> GetAll();
}
