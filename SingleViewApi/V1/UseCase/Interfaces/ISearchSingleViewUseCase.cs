using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface ISearchSingleViewUseCase
{
    SearchResponseObject Execute(string firstName, string lastName);
}
