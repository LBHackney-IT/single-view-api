using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetCombinedSearchResultsByNameUseCase : IGetCombinedSearchResultsByNameUseCase
{
    private IGetSearchResultsByNameUseCase _getSearchResultsByNameUseCase;
    private IGetJigsawCustomersUseCase _getJigsawCustomersUseCase;

    public GetCombinedSearchResultsByNameUseCase(IGetSearchResultsByNameUseCase getSearchResultsByNameUseCase, IGetJigsawCustomersUseCase getJigsawCustomersUseCase)
    {
        _getSearchResultsByNameUseCase = getSearchResultsByNameUseCase;
        _getJigsawCustomersUseCase = getJigsawCustomersUseCase;
    }

    public async Task<SearchResponseObject> Execute(string firstName, string lastName, int page, string userToken,
        string redisId)
    {
        var housingResults = await _getSearchResultsByNameUseCase.Execute(firstName, lastName, page, userToken);
        var jigsawResults = await _getJigsawCustomersUseCase.Execute(firstName, lastName, redisId);

        return housingResults;
    }

}
