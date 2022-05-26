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
        public async Task<AuthGatewayResponse> Execute(string redisKey, string hackneyToken)
        {
            var jigsawToken = _redisGateway.GetValue(hackneyToken);

            if (String.IsNullOrEmpty(jigsawToken))
            {
                var encyptedCredentials = _redisGateway.GetValue(redisKey);

                var credentials = _decoderHelper.DecodeJigsawCredentials(encyptedCredentials);

                var authGatewayResponse = await _jigsawGateway.GetAuthToken(credentials);

                _redisGateway.AddValueWithKey(hackneyToken, authGatewayResponse.Token, 1);

                return authGatewayResponse;
            }
            else
            {
                var authGatewayResponse = new AuthGatewayResponse
                {
                    Token = jigsawToken
                };

                return authGatewayResponse;
            }

        }
    }
}
