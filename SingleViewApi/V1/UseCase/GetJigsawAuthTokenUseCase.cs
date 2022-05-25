using System;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.Helpers;
using SingleViewApi.V1.Helpers.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetJigsawAuthTokenUseCase : IGetJigsawAuthTokenUseCase
    {
        private IJigsawGateway _jigsawGateway;
        private IRedisGateway _redisGateway;
        private IDecoderHelper _decoderHelper;

        public GetJigsawAuthTokenUseCase(IJigsawGateway jigsawGateway, IRedisGateway redisGateway, IDecoderHelper decoderHelper)
        {
            _jigsawGateway = jigsawGateway;
            _redisGateway = redisGateway;
            _decoderHelper = decoderHelper;

        }

        [LogCall]
        public async Task<AuthGatewayResponse> Execute(string redisKey)
        {
            //check if credentials exist in redis stored against hackneyToken


            var encyptedCredentials = _redisGateway.GetValue(redisKey);

            var credentials = _decoderHelper.DecodeJigsawCredentials(encyptedCredentials);

            var authGatewayResponse = await _jigsawGateway.GetAuthToken(credentials);

            //store auth token in redis, with hackney token as key

            return authGatewayResponse;

        }
    }
}
