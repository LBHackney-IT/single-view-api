using System.Threading.Tasks;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetJigsawAuthTokenUseCase
    {
        Task<string> Execute(string username);
    }
}
