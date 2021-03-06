using System;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.Gateways.Interfaces;
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

    [LogCall]
    public string Execute(string encryptedCredentials, string hackneyToken)
    {
        var decryptedCredentials = _decoderHelper.DecodeJigsawCredentials(encryptedCredentials);

        var authGatewayResponse = _jigsawGateway.GetAuthToken(decryptedCredentials).Result;

        if (String.IsNullOrEmpty(authGatewayResponse.Token)) return null;

        Console.WriteLine($"Adding Jigsaw token to Redis cache");

        _redisGateway.AddValueWithKey(hackneyToken, authGatewayResponse.Token, 1);

        Console.WriteLine("Jigsaw token added to Redis cache");

        var id = _redisGateway.AddValue(encryptedCredentials, 1);

        return id;
    }
}
