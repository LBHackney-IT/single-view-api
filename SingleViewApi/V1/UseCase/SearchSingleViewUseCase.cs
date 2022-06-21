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
    public class SearchSingleViewUseCase : ISearchSingleViewUseCase
    {
        private readonly ICustomerGateway _customerGateway;

        public SearchSingleViewUseCase(ICustomerGateway customerGateway)
        {
            _customerGateway = customerGateway;
        }

        [LogCall]

        public SearchResponseObject Execute(string firstName, string lastName)
        {
            var searchResults = _customerGateway.Search(firstName, lastName);

            var dataSourceName = "single-view";

            var singleViewId = new SystemId() { SystemName = dataSourceName, Id = $"{firstName} {lastName}" };

            var response = new SearchResponseObject() { SystemIds = new List<SystemId>() { singleViewId } };

            if (searchResults.Count == 0)
            {
                singleViewId.Error = SystemId.NotFoundMessage;
            }
            else
            {
                var personResults = new List<SearchResult>();

                foreach (var result in searchResults)
                {
                    var person = new SearchResult()
                    {
                        Id = result.Id.ToString(),
                        DataSource = dataSourceName,
                        FirstName = result.FirstName,
                        SurName = result.LastName,
                        DateOfBirth = result.DateOfBirth,
                        NiNumber = result.NiNumber,
                    };

                    personResults.Add(person);
                }

                response.SearchResponse = new SearchResponse()
                {

                    SearchResults = personResults,
                    Total = searchResults.Count
                };

            }

            return response;
        }
    }
}
