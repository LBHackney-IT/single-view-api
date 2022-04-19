using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.Person;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways
{
    public interface IPersonGateway
    {
        Task<Person> GetPersonById(int id, string userToken);
    }
}
