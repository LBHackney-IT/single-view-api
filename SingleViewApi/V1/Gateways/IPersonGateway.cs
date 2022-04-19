using System.Collections.Generic;
using Hackney.Shared.Person;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways
{
    public interface IPersonGateway
    {
        Person GetPersonById(int id, string userToken);
    }
}
