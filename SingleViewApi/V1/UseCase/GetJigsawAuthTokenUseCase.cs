using System;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.Helpers.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetJigsawAuthTokenUseCase : IGetJigsawAuthTokenUseCase
{
    private readonly IDecoderHelper _decoderHelper;
    private readonly IJigsawGateway _jigsawGateway;
    private readonly IRedisGateway _redisGateway;

    public GetJigsawAuthTokenUseCase(IJigsawGateway jigsawGateway, IRedisGateway redisGateway,
        IDecoderHelper decoderHelper)
    {
        _jigsawGateway = jigsawGateway;
        _redisGateway = redisGateway;
        _decoderHelper = decoderHelper;
    }

    [LogCall]
    public async Task<AuthGatewayResponse> Execute(string redisKey, string hackneyToken)
    {
        if (redisKey == "Placeholder-Jigsaw-Token")
            return new AuthGatewayResponse { Token = "Placeholder-Jigsaw-Token" };
        var jigsawToken = _redisGateway.GetValue(hackneyToken);

        if (string.IsNullOrEmpty(jigsawToken))
        {
            Console.WriteLine("No token found. Adding token...");

            var encryptedCredentials = _redisGateway.GetValue(redisKey);

            var credentials = _decoderHelper.DecodeJigsawCredentials(encryptedCredentials);

            var authGatewayResponse = await _jigsawGateway.GetAuthToken(credentials);

            _redisGateway.AddValueWithKey(hackneyToken, authGatewayResponse.Token, 1);

            return authGatewayResponse;
        }
        else
        {
            Console.WriteLine("Token found. Returning token...");

            var authGatewayResponse = new AuthGatewayResponse { Token = jigsawToken };

            return authGatewayResponse;
        }
    }
}
