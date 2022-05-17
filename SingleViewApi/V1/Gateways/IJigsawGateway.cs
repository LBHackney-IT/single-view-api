using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Helpers;

namespace SingleViewApi.V1.Gateways
{
    public interface IJigsawGateway
    {
        Task<string> GetAuthToken(JigsawCredentials credentials);
        Task<List<JigsawCustomerSearchApiResponseObject>> GetCustomers(string firstName, string lastName, string bearerToken);
    }
}
