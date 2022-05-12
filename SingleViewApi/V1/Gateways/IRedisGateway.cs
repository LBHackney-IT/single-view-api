using System.Threading.Tasks;

namespace SingleViewApi.V1.Gateways
{
    public interface IRedisGateway
    {
        string AddValue(string value, int ttlDays);
        string GetValue(string id);
    }
}
