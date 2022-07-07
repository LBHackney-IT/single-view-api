using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways.Interfaces;

public interface IEqualityInformationGateway
{
    public Task<EqualityInformationResponseObject> GetEqualityInformationById(string id, string userToken);
}
