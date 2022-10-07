using System;
using Hackney.Core.Logging;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.Helpers.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class StoreJigsawCredentialsUseCase : IStoreJigsawCredentialsUseCase
{
    private readonly IDecoderHelper _decoderHelper;
    private readonly IJigsawGateway _jigsawGateway;
    private readonly IRedisGateway _redisGateway;

    public StoreJigsawCredentialsUseCase(IRedisGateway redisGateway, IJigsawGateway jigsawGateway,
        IDecoderHelper decoderHelper)
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

        if (string.IsNullOrEmpty(authGatewayResponse.Token)) return null;

        Console.WriteLine("Adding Jigsaw token to Redis cache");

        _redisGateway.AddValueWithKey(hackneyToken, authGatewayResponse.Token, 1);

        Console.WriteLine("Jigsaw token added to Redis cache");

        var id = _redisGateway.AddValue(encryptedCredentials, 1);

        return id;
    }
}
