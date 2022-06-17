using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary;
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
    public async Task<SearchResponseObject> Execute(string firstName, string lastName, string userToken,
        string redisId)
    {
        var housingResults = await _getSearchResultsByNameUseCase.Execute(firstName, lastName, userToken);
        int total = 0;

        List<SearchResult> concatenatedResults;
        List<SystemId> jigsawids = new List<SystemId>();

        if (redisId != null)
        {
            var jigsawResults = await _getJigsawCustomersUseCase.Execute(firstName, lastName, redisId, userToken);
            concatenatedResults = ConcatenateResults(housingResults?.SearchResponse?.SearchResults,
                jigsawResults?.SearchResponse?.SearchResults);
            total += jigsawResults?.SearchResponse?.Total ?? 0;

            jigsawids = jigsawResults?.SystemIds ?? new List<SystemId>();
        }
        else
        {
            concatenatedResults = ConcatenateResults(housingResults?.SearchResponse?.SearchResults);
        }

        var sortedResults = SortResultsByRelevance(firstName, lastName, concatenatedResults);


        total += housingResults?.SearchResponse?.Total ?? 0;


        var collatedResults = new SearchResponseObject
        {
            SearchResponse = new SearchResponse
            {
                SearchResults = sortedResults,
                Total = total
            },
            SystemIds = housingResults?.SystemIds?.Concat(jigsawids).ToList()
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
        string firstNameFormatted = Normalise(firstName);
        string lastNameFormatted = Normalise(lastName);
        int firstNameFormattedLength = firstNameFormatted.Length;
        int lastNameFormattedLength = lastNameFormatted.Length;
        return searchResults

            // Compare normalised substrings of the search results
            .OrderBy(o => !String.Equals(Normalise(o.FirstName, firstNameFormattedLength), firstName, StringComparison.CurrentCultureIgnoreCase))
            .ThenBy(o => !String.Equals(Normalise(o.SurName, lastNameFormattedLength), lastName, StringComparison.CurrentCultureIgnoreCase))
            .ThenBy(o => !String.Equals(Normalise(o.FirstName, firstNameFormattedLength), firstName, StringComparison.CurrentCultureIgnoreCase) &&
                        !String.Equals(Normalise(o.SurName, lastNameFormattedLength), lastName, StringComparison.CurrentCultureIgnoreCase))

            // Then compare normalised whole matches
            .ThenBy(o => !String.Equals(Normalise(o.FirstName), firstName, StringComparison.CurrentCultureIgnoreCase))
            .ThenBy(o => !String.Equals(Normalise(o.SurName), lastName, StringComparison.CurrentCultureIgnoreCase))
            .ThenBy(o => !String.Equals(Normalise(o.FirstName), firstName, StringComparison.CurrentCultureIgnoreCase) &&
                         !String.Equals(Normalise(o.SurName), lastName, StringComparison.CurrentCultureIgnoreCase))

            // Then compare non normalised whole matches
            .ThenBy(o => !String.Equals(o.FirstName, firstName, StringComparison.CurrentCultureIgnoreCase))
            .ThenBy(o => !String.Equals(o.SurName, lastName, StringComparison.CurrentCultureIgnoreCase))
            .ThenBy(o => !String.Equals(o.FirstName, firstName, StringComparison.CurrentCultureIgnoreCase) &&
                         !String.Equals(Normalise(o.SurName), lastName, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
    }

    private string Normalise(string value, int len = 0)
    {
        value = value.Replace(" ", "");
        if (len > 0 && len <= value.Length) value = value.Substring(0, len);
        return value;
    }
}
