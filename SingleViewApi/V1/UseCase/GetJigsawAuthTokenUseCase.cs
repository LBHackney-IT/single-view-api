using System;
using System.Threading.Tasks;
using Hackney.Core.Logging;
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
        public async Task<string> Execute(string redisKey)
        {

            Console.WriteLine($"------> GetJigsawAuthTokenUseCase - Getting Value From Redis: Key = {redisKey}");

            var encyptedCredentials = _redisGateway.GetValue(redisKey);

            Console.WriteLine($"------> Got Value From Redis: Key = {redisKey}. Decoding...");

            var credentials = _decoderHelper.DecodeJigsawCredentials(encyptedCredentials);

            Console.WriteLine($"------> Decoded Value From Redis: Key = {redisKey}. Getting Token...");

            var token = await _jigsawGateway.GetAuthToken(credentials);

            Console.WriteLine($"------> Got Token From Redis: Key = {redisKey}. Saving Token...");

            return token;

        }
    }
}
