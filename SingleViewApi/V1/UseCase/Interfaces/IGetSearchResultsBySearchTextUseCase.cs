using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetSearchResultsBySearchTextUseCase
    {
        Task<SearchResponseObject> Execute(string searchText, string userToken);
    }
}
