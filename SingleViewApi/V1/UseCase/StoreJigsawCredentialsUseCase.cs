using System.Threading.Tasks;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.Helpers.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class StoreJigsawCredentialsUseCase : IStoreJigsawCredentialsUseCase
{
    private IRedisGateway _redisGateway;
    private IJigsawGateway _jigsawGateway;
    private IDecoderHelper _decoderHelper;

    public StoreJigsawCredentialsUseCase(IRedisGateway redisGateway, IJigsawGateway jigsawGateway, IDecoderHelper decoderHelper)
    {
        _redisGateway = redisGateway;
        _jigsawGateway = jigsawGateway;
        _decoderHelper = decoderHelper;
    }

    public string Execute(string encryptedCredentials)
    {

        var decryptedCredentials = _decoderHelper.DecodeJigsawCredentials(encryptedCredentials);

        var token = _jigsawGateway.GetAuthToken(decryptedCredentials);

        if (token == null) return null;

        var id = _redisGateway.AddValue(encryptedCredentials, 1);

        return id;
    }
}
