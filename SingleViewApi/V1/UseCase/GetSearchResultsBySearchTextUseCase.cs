using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        //[LogCall]

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
                var personResults = new List<Person>();

                foreach (var person in searchResults.Results.Persons)
                {
                    personResults.Add(person);
                }

                response.SearchResponse = new SearchResponse() { Response = new HousingSearchApiResponse()
                {
                    Results = new Results()
                    {
                        Persons = personResults
                    },
                    Total = searchResults.Total
                } };



            }

            return response;
        }
    }
}
