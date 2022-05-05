using System.Threading.Tasks;

namespace SingleViewApi.V1.Gateways
{
    public interface IJigsawGateway
    {
        Task GetAuthToken();
    }
}
