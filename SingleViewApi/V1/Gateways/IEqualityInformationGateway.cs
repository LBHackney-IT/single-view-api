using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways;

public interface IEqualityInformationGateway
{
    Task<EqualityInformationResponseObject> GetEqualityInformationById(string id, string userToken);
}
