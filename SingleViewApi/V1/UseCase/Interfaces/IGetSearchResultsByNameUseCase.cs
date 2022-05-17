using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetSearchResultsByNameUseCase
    {
        Task<SearchResponseObject> Execute(string searchText, int page, string userToken);
    }
}
