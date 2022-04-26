using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.Person;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways
{
    public interface IHousingSearchGateway
    {
        Task<HousingSearchApiResponse> GetSearchResultsBySearchText(string searchText,int page, string userToken);
    }
}
