using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using Hackney.Shared.Person.Domain;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetSearchResultsByNameUseCase : IGetSearchResultsByNameUseCase
    {
        private IHousingSearchGateway _housingSearchGateway;


        public GetSearchResultsByNameUseCase(IHousingSearchGateway housingSearchGateway)
        {
            _housingSearchGateway = housingSearchGateway;
        }

        [LogCall]

        public async Task<SearchResponseObject> Execute(string firstName, string lastName, int page, string userToken)
        {
            var searchText = $"{firstName}+{lastName}";

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
                    var personTypes = new List<PersonType>();
                    if (result.PersonTypes != null)
                    {
                        personTypes = result.PersonTypes.ToList();
                    }
                    var person = new SearchResult()
                    {
                        Id = result.Id,
                        DataSource = DataSource.HousingSearch,
                        FirstName = result.FirstName,
                        SurName = result.Surname,
                        Title = result.Title,
                        PreferredFirstName = result.PreferredFirstName,
                        PreferredSurname = result.PreferredSurname,
                        MiddleName = result.MiddleName,
                        PersonTypes = personTypes,
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

                    personResults.Add(person);
                }

                response.SearchResponse = new SearchResponse()
                {

                    SearchResults = personResults,
                    Total = searchResults.Total
                };

            }

            return response;
        }
    }
}
