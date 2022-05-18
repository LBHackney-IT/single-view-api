using System;
using System.Collections.Generic;
using System.Linq;
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

        var collatedResults = new SearchResponseObject()
        {
            SearchResponse = new SearchResponse()
            {
                SearchResults = ConcatenateResults(housingResults.SearchResponse.SearchResults, jigsawResults.SearchResponse.SearchResults),

                Total = housingResults.SearchResponse.Total + jigsawResults.SearchResponse.Total
            },
            SystemIds = housingResults.SystemIds.Concat(jigsawResults.SystemIds).ToList()
        };

        return collatedResults;
    }

    private List<SearchResult> ConcatenateResults(List<SearchResult> housingResults, List<SearchResult> jigsawResults)
    {
        if (housingResults == null && jigsawResults == null)
        {
            return new List<SearchResult>();
        }
        else if (housingResults == null)
        {
            return jigsawResults;
        }
        else if (jigsawResults == null)
        {
            return housingResults;
        }
        return housingResults.Concat(jigsawResults).ToList();
    }
}
