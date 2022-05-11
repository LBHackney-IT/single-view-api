using System.Threading.Tasks;

namespace SingleViewApi.V1.Gateways
{
    public interface IRedisGateway
    {
        string DoTheThing(string input);
    }
}
