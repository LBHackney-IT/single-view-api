using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetSearchResultsBySearchTextUseCase : IGetSearchResultsBySearchTextUseCase
    {
        private IHousingSearchGateway _housingSearchGateway;

        public GetSearchResultsBySearchTextUseCase(IHousingSearchGateway housingSearchGateway)
        {
            _housingSearchGateway = housingSearchGateway;
        }

        [LogCall]

        public async Task<SearchResponseObject> Execute(string searchText, int page, string userToken)
        {
            var searchResults = await _housingSearchGateway.GetSearchResultsBySearchText(searchText, page, userToken);

            var housingSearchApiId = new SystemId() { SystemName = "HousingSearchApi", Id = searchText };

            var response = new SearchResponseObject() { SystemIds = new List<SystemId>() { housingSearchApiId } };

            if (searchResults == null)
            {
                housingSearchApiId.Error = "No results found";
            }
            else
            {
                var personResults = new List<SearchResult>();

                foreach (var result in searchResults.Results.Persons)
                {
                    Console.WriteLine("Looping through search results - total results {0}", searchResults.Total);
                    var person = new SearchResult()
                    {
                        Id = result.Id,
                        FirstName = result.Firstname,
                        SurName = result.Surname,
                        Title = result.Title,
                        PreferredFirstName = result.PreferredFirstname,
                        PreferredSurname = result.PreferredSurname,
                        MiddleName = result.MiddleName,
                        PersonTypes = result.PersonTypes,
                        DateOfBirth = result.DateOfBirth,
                        KnownAddresses = new List<KnownAddress>(result.Tenures.Select(t => new KnownAddress()
                        {
                            Id = t.Id,
                            CurrentAddress = t.IsActive,
                            StartDate = t.StartDate,
                            EndDate = t.EndDate,
                            FullAddress = t.AssetFullAddress
                        }))
                    };

                    Console.WriteLine("The person being added is {0}",person);

                    personResults.Add(person);
                }

                response.SearchResponse = new SearchResponse()
                {

                    SearchResults = personResults, Total = searchResults.Total
                };

            }

            Console.WriteLine(response);

            return response;
        }
    }
}
