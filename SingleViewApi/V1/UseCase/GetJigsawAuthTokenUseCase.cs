using System;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetJigsawAuthTokenUseCase : IGetJigsawAuthTokenUseCase
    {
        private IJigsawGateway _jigsawGateway;
        private IRedisGateway _redisGateway;

        public GetJigsawAuthTokenUseCase(IJigsawGateway jigsawGateway, IRedisGateway redisGateway)
        {
            _jigsawGateway = jigsawGateway;
            _redisGateway = redisGateway;
        }

        [LogCall]

        public async Task<string> Execute(string hashedUsername, string hashedPassword)
        {
            //logic to encrypt token here
            var email = hashedUsername;
            var password = hashedPassword;

            var token = await _jigsawGateway.GetAuthToken(email, password );

            var id = _redisGateway.AddValue(token, 1);
            return id;

        }
    }
}
