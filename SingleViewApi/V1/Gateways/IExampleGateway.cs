using System.Collections.Generic;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);

        List<Entity> GetAll();
    }
}
