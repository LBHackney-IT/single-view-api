using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Hackney.Core.Logging;
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
    [LogCall]
    public async Task<SearchResponseObject> Execute(string firstName, string lastName, int page, string userToken,
        string redisId)
    {
        var housingResults = await _getSearchResultsByNameUseCase.Execute(firstName, lastName, page, userToken);
        var jigsawResults = await _getJigsawCustomersUseCase.Execute(firstName, lastName, redisId);

        var concatenatedResults = ConcatenateResults(housingResults.SearchResponse.SearchResults,
            jigsawResults.SearchResponse.SearchResults);

        var sortedResults = SortResultsByRelevance(firstName, lastName, concatenatedResults);


        var collatedResults = new SearchResponseObject()

        {
            SearchResponse = new SearchResponse()
            {
                SearchResults = sortedResults,

                Total = housingResults.SearchResponse.Total + jigsawResults.SearchResponse.Total
            },
            SystemIds = housingResults.SystemIds.Concat(jigsawResults.SystemIds).ToList()
        };

        return collatedResults;
    }

    [LogCall]
    public List<SearchResult> ConcatenateResults([Optional] List<SearchResult> housingResults, [Optional] List<SearchResult> jigsawResults)
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

    [LogCall]
    public List<SearchResult> SortResultsByRelevance(string firstName, string lastName, List<SearchResult> searchResults)
    {
        return searchResults.OrderBy(x => x.FirstName == firstName ? 0 : 1).ThenBy(x => x.SurName == lastName ? 0 : 1).ToList();
    }
}
