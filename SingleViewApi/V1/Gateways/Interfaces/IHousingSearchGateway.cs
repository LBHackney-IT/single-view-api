using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways.Interfaces;

public interface IHousingSearchGateway
{
    Task<HousingSearchApiResponse> GetSearchResultsBySearchText(string searchText, string userToken);
}
