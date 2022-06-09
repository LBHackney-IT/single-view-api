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
using SingleViewApi.V1.Helpers.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetJigsawCustomersUseCase : IGetJigsawCustomersUseCase
    {
        private readonly IJigsawGateway _jigsawGateway;
        private readonly IGetJigsawAuthTokenUseCase _getJigsawAuthTokenUseCase;
        private readonly IDataSourceGateway _dataSourceGateway;

        public GetJigsawCustomersUseCase(IJigsawGateway jigsawGateway, IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase, IDataSourceGateway dataSourceGateway)
        {
            _jigsawGateway = jigsawGateway;
            _getJigsawAuthTokenUseCase = getJigsawAuthTokenUseCase;
            _dataSourceGateway = dataSourceGateway;
        }


        [LogCall]

        public async Task<SearchResponseObject> Execute(string firstName, string lastName, string redisId, string hackneyToken)
        {
            var authGatewayResponse = _getJigsawAuthTokenUseCase.Execute(redisId, hackneyToken).Result;

            var dataSource = _dataSourceGateway.GetEntityById(2);

            var jigsawApiId = new SystemId() { SystemName = dataSource.Name, Id = $"{firstName}+{lastName}" };

            var response = new SearchResponseObject() { SystemIds = new List<SystemId>() { jigsawApiId } };

            if (!String.IsNullOrEmpty(authGatewayResponse.ExceptionMessage))
            {
                jigsawApiId.Error = authGatewayResponse.ExceptionMessage;
                return response;
            }

            var searchResults = await _jigsawGateway.GetCustomers(firstName, lastName, authGatewayResponse.Token);

            if (searchResults == null)
            {
                jigsawApiId.Error = SystemId.NotFoundMessage;
                return response;
            }

            var personResults = new List<SearchResult>();

            foreach (var result in searchResults)
            {
                var person = new SearchResult()
                {
                    Id = result.Id,
                    DataSource = dataSource.Name,
                    FirstName = result.FirstName,
                    SurName = result.LastName,
                    DateOfBirth = result.DoB,
                    NiNumber = result.NiNumber,
                    KnownAddresses = new List<KnownAddress>()
                    {
                        new KnownAddress()
                        {
                            CurrentAddress = true,
                            FullAddress = result.Address
                        }
                    },

                };

                personResults.Add(person);
            }

            response.SearchResponse = new SearchResponse()
            {
                SearchResults = personResults,
                Total = personResults.Count
            };


            return response;
        }
    }
}
