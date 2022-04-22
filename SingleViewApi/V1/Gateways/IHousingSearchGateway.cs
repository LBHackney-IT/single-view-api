using System.Threading.Tasks;

namespace SingleViewApi.V1.Gateways
{
    public interface IHousingSearchGateway
    {
        Task<dynamic> SearchBySearchText(string searchText, string userToken);
    }
}
