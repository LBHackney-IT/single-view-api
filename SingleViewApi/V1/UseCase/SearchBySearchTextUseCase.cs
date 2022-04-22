using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Hackney.Core.Logging;
using Hackney.Shared.Person;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class SearchBySearchTextUseCase : ISearchBySearchTextUseCase
    {
        private IHousingSearchGateway _housingSearchGateway;

        public SearchBySearchTextUseCase(IHousingSearchGateway housingSearchGateway)
        {
            _housingSearchGateway = housingSearchGateway;
        }

        [LogCall]

        public async Task<SearchResponseObject> Execute(string searchText, string userToken)
        {
            var searchResults = await _housingSearchGateway.SearchBySearchText(searchText, userToken);

            var housingSearchApiId = new SystemId() { SystemName = "HousingSearchApi", Id = searchText };

            var response = new SearchResponseObject() { SystemIds = new List<SystemId>() { housingSearchApiId } };

            if (searchResults == null)
            {
                housingSearchApiId.Error = "No results found";
            }
            else
            {
                response.SearchResponse = response.SearchResponse;

                //logic here to create list of housing search results
            }

            return response;
        }
    }
}
