using System.Threading.Tasks;

namespace SingleViewApi.V1.Gateways
{
    public interface IRedisGateway
    {
        dynamic DoTheThing(string input);
    }
}
