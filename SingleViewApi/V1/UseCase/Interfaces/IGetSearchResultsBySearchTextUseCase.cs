using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetSearchResultsBySearchTextUseCase
    {
        Task<HousingSearchApiResponseObject> Execute(string searchText, string userToken);
    }
}
