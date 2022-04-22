using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.Person;

namespace SingleViewApi.V1.Gateways
{
    public interface IHousingSearchGateway
    {
        Task<List<Person>> SearchBySearchText(string searchText, string userToken);
    }
}
