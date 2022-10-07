using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface ISearchSingleViewUseCase
    {
        SearchResponseObject Execute(string firstName, string lastName);
    }
}
