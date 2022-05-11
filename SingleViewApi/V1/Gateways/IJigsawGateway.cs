using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways
{
    public interface IJigsawGateway
    {
        Task<string> GetAuthToken(string email, string password);
        Task<List<JigsawCustomerSearchApiResponseObject>> GetCustomers(string firstName, string lastName, string bearerToken);
    }
}
