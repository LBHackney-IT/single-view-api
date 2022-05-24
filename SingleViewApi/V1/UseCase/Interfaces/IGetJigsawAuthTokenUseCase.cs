using System.Threading.Tasks;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetJigsawAuthTokenUseCase
    {
        Task<AuthGatewayResponse> Execute(string redisKey);
    }
}
