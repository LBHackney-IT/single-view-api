using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using Hackney.Shared.Person.Domain;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetSearchResultsByNameUseCase : IGetSearchResultsByNameUseCase
    {
        private IHousingSearchGateway _housingSearchGateway;
        private readonly IDataSourceGateway _dataSourceGateway;


        public GetSearchResultsByNameUseCase(IHousingSearchGateway housingSearchGateway, IDataSourceGateway dataSourceGateway)
        {
            _housingSearchGateway = housingSearchGateway;
            _dataSourceGateway = dataSourceGateway;
        }

        [LogCall]

        public async Task<SearchResponseObject> Execute(string firstName, string lastName, string userToken)
        {
            var searchText = $"{firstName}+{lastName}";

            var searchResults = await _housingSearchGateway.GetSearchResultsBySearchText(searchText, userToken);

            var dataSource = _dataSourceGateway.GetEntityById(1);

            var housingSearchApiId = new SystemId() { SystemName = dataSource.Name, Id = searchText };

            var response = new SearchResponseObject() { SystemIds = new List<SystemId>() { housingSearchApiId } };

            if (searchResults == null)
            {
                housingSearchApiId.Error = SystemId.NotFoundMessage;
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
                        Id = result.Id.ToString(),
                        DataSource = dataSource,
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
