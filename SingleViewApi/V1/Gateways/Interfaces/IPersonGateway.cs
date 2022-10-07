using System.Threading.Tasks;
using Hackney.Shared.Person;

namespace SingleViewApi.V1.Gateways.Interfaces
{
    public interface IPersonGateway
    {
        Task<Person> GetPersonById(string id, string userToken);
    }
}
