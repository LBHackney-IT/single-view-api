using System.Collections.Generic;
using System.Linq;
using Hackney.Core.Logging;
using ServiceStack;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class SearchSingleViewUseCase : ISearchSingleViewUseCase
    {
        private readonly ICustomerGateway _customerGateway;
        private readonly IDataSourceGateway _dataSourceGateway;

        public SearchSingleViewUseCase(ICustomerGateway customerGateway, IDataSourceGateway dataSourceGateway)
        {
            _customerGateway = customerGateway;
            _dataSourceGateway = dataSourceGateway;
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
                var dataSources = _dataSourceGateway.GetAll();
                var personResults = searchResults.Select(result => new SearchResult()
                {
                    Id = result.Id.ToString(),
                    DataSources = result.DataSources.Map(customerDataSource => dataSources.Find(dataSource => dataSource.Id == customerDataSource.DataSourceId)?.Name),
                    FirstName = result.FirstName,
                    SurName = result.LastName,
                    DateOfBirth = result.DateOfBirth,
                    NiNumber = result.NiNumber
                }
                ).ToList();

                response.SearchResponse = new SearchResponse()
                {
                    UngroupedResults = personResults,
                    Total = searchResults.Count
                };

                foreach (var searchResponseUngroupedResult in response.SearchResponse.UngroupedResults)
                {
                    searchResponseUngroupedResult.IsMergedSingleViewRecord = true;
                }

            }

            return response;
        }
    }
}
