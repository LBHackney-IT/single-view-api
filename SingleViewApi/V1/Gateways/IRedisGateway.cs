using System.Threading.Tasks;

namespace SingleViewApi.V1.Gateways
{
    public interface IRedisGateway
    {
        string AddValue(string value);
        string GetValue(string id);
    }
}
