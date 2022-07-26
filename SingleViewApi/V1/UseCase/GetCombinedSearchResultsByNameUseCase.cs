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

    private readonly IGetSearchResultsByNameUseCase _getSearchResultsByNameUseCase;
    private readonly IGetJigsawCustomersUseCase _getJigsawCustomersUseCase;
    private readonly ISearchSingleViewUseCase _searchSingleViewUseCase;
    private readonly IGetCouncilTaxAccountsByCustomerNameUseCase _getCouncilTaxAccountsByCustomerNameUseCase;
    private readonly IGetHousingBenefitsAccountsByCustomerNameUseCase _getHousingBenefitsAccountsByCustomerNameUseCase;

    public GetCombinedSearchResultsByNameUseCase(IGetSearchResultsByNameUseCase getSearchResultsByNameUseCase, IGetJigsawCustomersUseCase getJigsawCustomersUseCase, ISearchSingleViewUseCase searchSingleViewUseCase, IGetCouncilTaxAccountsByCustomerNameUseCase getCouncilTaxAccountsByCustomerNameUseCase, IGetHousingBenefitsAccountsByCustomerNameUseCase getHousingBenefitsAccountsByCustomerNameUseCase)
    {
        _getSearchResultsByNameUseCase = getSearchResultsByNameUseCase;
        _searchSingleViewUseCase = searchSingleViewUseCase;
        _getJigsawCustomersUseCase = getJigsawCustomersUseCase;
        _getCouncilTaxAccountsByCustomerNameUseCase = getCouncilTaxAccountsByCustomerNameUseCase;
        _getHousingBenefitsAccountsByCustomerNameUseCase = getHousingBenefitsAccountsByCustomerNameUseCase;
    }

    [LogCall]
    public async Task<SearchResponseObject> Execute(string firstName, string lastName, string userToken, string redisId, string dateOfBirth)
    {
        var singleViewResults = _searchSingleViewUseCase.Execute(firstName, lastName);
        var housingResults = await _getSearchResultsByNameUseCase.Execute(firstName, lastName, userToken);
        var councilTaxResults =
          await _getCouncilTaxAccountsByCustomerNameUseCase.Execute(firstName, lastName, userToken);
        var housingBenefitsResults =
            await _getHousingBenefitsAccountsByCustomerNameUseCase.Execute(firstName, lastName, userToken);

        int total = singleViewResults?.SearchResponse?.Total ?? 0;

        List<SearchResult> concatenatedResults;
        List<SystemId> jigsawIds = new List<SystemId>();

        if (redisId != null)
        {
            var jigsawResults = await _getJigsawCustomersUseCase.Execute(firstName, lastName, redisId, userToken);

            concatenatedResults = ConcatenateResults(
                singleViewResults: singleViewResults?.SearchResponse?.UngroupedResults,
                housingResults: housingResults?.SearchResponse?.UngroupedResults,
                jigsawResults: jigsawResults?.SearchResponse?.UngroupedResults,
                councilTaxResults: councilTaxResults?.SearchResponse?.UngroupedResults,
                housingBenefitsResults: housingBenefitsResults?.SearchResponse?.UngroupedResults
                );

            total += jigsawResults?.SearchResponse?.Total ?? 0;

            jigsawIds = jigsawResults?.SystemIds ?? new List<SystemId>();
        }
        else
        {
            concatenatedResults = ConcatenateResults(
                singleViewResults: singleViewResults?.SearchResponse?.UngroupedResults,
                housingResults: housingResults?.SearchResponse?.UngroupedResults,
                councilTaxResults: councilTaxResults?.SearchResponse?.UngroupedResults,
                housingBenefitsResults: housingBenefitsResults?.SearchResponse?.UngroupedResults
                );
        }

        var sortedResults = SortResultsByRelevance(firstName, lastName, concatenatedResults);
        total += housingResults?.SearchResponse?.Total ?? 0;
        total += councilTaxResults?.SearchResponse?.Total ?? 0;
        total += housingBenefitsResults?.SearchResponse?.Total ?? 0;

        var systemIds = singleViewResults?.SystemIds?.Concat(housingResults?.SystemIds)
            .Concat(councilTaxResults?.SystemIds).Concat(housingBenefitsResults?.SystemIds).ToList();

        var groupedResults = GroupByRelevance(firstName, lastName, dateOfBirth, sortedResults);

        var ungroupedResults = RemoveDuplicates(groupedResults, sortedResults);

        var collatedResults = new SearchResponseObject
        {
            SearchResponse = new SearchResponse
            {
                UngroupedResults = ungroupedResults,
                Total = total,
                GroupedResults = groupedResults
            },
            SystemIds = systemIds.Concat(jigsawIds).ToList()
        };

        return collatedResults;
    }

    [LogCall]
    private List<SearchResult> ConcatenateResults([Optional] List<SearchResult> singleViewResults, [Optional] List<SearchResult> housingResults, [Optional] List<SearchResult> jigsawResults, [Optional] List<SearchResult> councilTaxResults, [Optional] List<SearchResult> housingBenefitsResults)
    {
        return NeverNull(singleViewResults)
            .Concat(NeverNull(housingResults))
            .Concat(NeverNull(jigsawResults))
            .Concat(NeverNull(councilTaxResults))
            .Concat(NeverNull(housingBenefitsResults))
            .ToList();
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

    [LogCall]
    public List<SearchResult> GroupByRelevance(string firstName, string lastName, string dateOfBirth, List<SearchResult> searchResults)
    {
        var groupedByName = searchResults.Where(s => s.FirstName == firstName && s.SurName == lastName).ToList();

        if (!string.IsNullOrEmpty(dateOfBirth))
        {
            return groupedByName.Where(s => s.DateOfBirth?.ToString("dd/MM/yyyy") == dateOfBirth).ToList();
        }

        return groupedByName;
    }

    [LogCall]
    public List<SearchResult> RemoveDuplicates(List<SearchResult> groupedResults, List<SearchResult> searchResults)
    {
        return searchResults.Where(s => groupedResults.All(g => g.Id != s.Id)).ToList();
    }

    private string Normalise(string value, int len = 0)
    {
        value = value.Replace(" ", "");
        if (len > 0 && len <= value.Length) value = value.Substring(0, len);
        return value;
    }

    private IEnumerable<T> NeverNull<T>(IEnumerable<T> value)
    {
        return value ?? Enumerable.Empty<T>();
    }
}
