using System;
using System.Threading.Tasks;
using Hackney.Core.Logging;
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

    [LogCall]
    public string Execute(string encryptedCredentials)
    {
        Console.WriteLine("--------- USECASE HIT ---------");

        var decryptedCredentials = _decoderHelper.DecodeJigsawCredentials(encryptedCredentials);

        Console.WriteLine("--------- DECODED ---------");

        var token = _jigsawGateway.GetAuthToken(decryptedCredentials).Result;

        Console.WriteLine("--------- GOT TOKEN ---------");

        if (String.IsNullOrEmpty(token)) return null;

        var id = _redisGateway.AddValue(encryptedCredentials, 1);

        Console.WriteLine("--------- GOT ID ---------");

        return id;
    }
}
