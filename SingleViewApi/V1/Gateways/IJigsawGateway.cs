using System.Threading.Tasks;

namespace SingleViewApi.V1.Gateways
{
    public interface IJigsawGateway
    {
        Task<string> GetAuthToken(string email, string password);
        Task<dynamic> GetCustomers(string firstName, string lastName, string bearerToken);
    }
}
