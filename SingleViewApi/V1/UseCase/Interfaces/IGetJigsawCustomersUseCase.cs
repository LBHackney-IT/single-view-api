using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetJigsawCustomersUseCase
    {
        Task<SearchResponseObject> Execute(string firstName, string lastName, string redisId, string hackneyToken);
    }
}
