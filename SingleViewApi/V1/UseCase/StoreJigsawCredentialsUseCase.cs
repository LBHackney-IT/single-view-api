using System.Threading.Tasks;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class StoreJigsawCredentialsUseCase : IStoreJigsawCredentialsUseCase
{
    private IRedisGateway _redisGateway;

    public StoreJigsawCredentialsUseCase(IRedisGateway redisGateway)
    {
        _redisGateway = redisGateway;
    }

    public string Execute(string jwt)
    {
        //decrypt credentials and authorise
        // if returns a token, store id. if not, return null
        var id = _redisGateway.AddValue(jwt, 1);

        return id;
    }
}
