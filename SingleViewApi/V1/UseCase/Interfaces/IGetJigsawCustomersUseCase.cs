using System.Threading.Tasks;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetJigsawCustomersUseCase
    {
        Task<dynamic> Execute(string firstName, string lastName, string bearerToken);
    }
}
