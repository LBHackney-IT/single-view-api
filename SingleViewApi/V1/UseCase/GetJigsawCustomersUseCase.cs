using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using Hackney.Shared.Person.Domain;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.Helpers.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetJigsawCustomersUseCase : IGetJigsawCustomersUseCase
    {
        private IJigsawGateway _jigsawGateway;
        private IGetJigsawAuthTokenUseCase _getJigsawAuthTokenUseCase;

        public GetJigsawCustomersUseCase(IJigsawGateway jigsawGateway, IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase)
        {
            _jigsawGateway = jigsawGateway;
            _getJigsawAuthTokenUseCase = getJigsawAuthTokenUseCase;
        }


        [LogCall]

        public async Task<SearchResponseObject> Execute(string firstName, string lastName, string redisId, string hackneyToken)
        {
            var authGatewayResponse = _getJigsawAuthTokenUseCase.Execute(redisId, hackneyToken).Result;

            var jigsawApiId = new SystemId() { SystemName = "Jigsaw", Id = $"{firstName}+{lastName}" };

            var response = new SearchResponseObject() { SystemIds = new List<SystemId>() { jigsawApiId } };

            if (!String.IsNullOrEmpty(authGatewayResponse.ExceptionMessage))
            {
                jigsawApiId.Error = authGatewayResponse.ExceptionMessage;
                return response;
            }

            var searchResults = await _jigsawGateway.GetCustomers(firstName, lastName, authGatewayResponse.Token);

            if (searchResults == null)
            {
                jigsawApiId.Error = "No results found";
                return response;
            }

            var personResults = new List<SearchResult>();

            foreach (var result in searchResults)
            {
                var person = new SearchResult()
                {
                    Id = result.Id,
                    DataSource = DataSource.Jigsaw,
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
