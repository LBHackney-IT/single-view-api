using System.Threading.Tasks;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class StoreJigsawCredentialsUseCase : IStoreJigsawCredentialsUseCase
{
    private IRedisGateway _redisGateway;
    private IJigsawGateway _jigsawGateway;

    public StoreJigsawCredentialsUseCase(IRedisGateway redisGateway, IJigsawGateway jigsawGateway)
    {
        _redisGateway = redisGateway;
        _jigsawGateway = jigsawGateway;
    }

    public string Execute(string jwt)
    {

        var username = "test";
        var password = "test";

        var token = _jigsawGateway.GetAuthToken(username, password);

        if (token == null) return null;

        var id = _redisGateway.AddValue(jwt, 1);

        return id;
    }
}
