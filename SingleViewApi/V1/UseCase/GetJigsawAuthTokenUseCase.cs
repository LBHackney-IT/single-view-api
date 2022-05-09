using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetJigsawAuthTokenUseCase : IGetJigsawAuthTokenUseCase
    {
        private IJigsawGateway _jigsawGateway;

        public GetJigsawAuthTokenUseCase(IJigsawGateway jigsawGateway)
        {
            _jigsawGateway = jigsawGateway;
        }

        [LogCall]

        public async Task<string> Execute(string username)
        {
            var token = await _jigsawGateway.GetAuthToken(username);

            return token;
        }
    }
}
