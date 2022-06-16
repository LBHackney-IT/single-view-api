using System.Threading.Tasks;

namespace SingleViewApi.V1.Gateways;

public interface IEqualityInformationGateway
{
    Task<dynamic> GetEqualityInformationById(string id, string userToken);
}
