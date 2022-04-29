using System.Threading.Tasks;
using Hackney.Shared.Person;

namespace SingleViewApi.V1.Gateways
{
    public interface IPersonGateway
    {
        Task<Person> GetPersonById(string id, string userToken);
    }
}
