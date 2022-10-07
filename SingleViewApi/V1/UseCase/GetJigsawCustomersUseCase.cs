using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetJigsawCustomersUseCase : IGetJigsawCustomersUseCase
{
    private readonly IDataSourceGateway _dataSourceGateway;
    private readonly IGetJigsawAuthTokenUseCase _getJigsawAuthTokenUseCase;
    private readonly IJigsawGateway _jigsawGateway;

    public GetJigsawCustomersUseCase(IJigsawGateway jigsawGateway, IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase,
        IDataSourceGateway dataSourceGateway)
    {
        _jigsawGateway = jigsawGateway;
        _getJigsawAuthTokenUseCase = getJigsawAuthTokenUseCase;
        _dataSourceGateway = dataSourceGateway;
    }


    [LogCall]
    public async Task<SearchResponseObject> Execute(string firstName, string lastName, string redisId,
        string hackneyToken)
    {
        var authGatewayResponse = _getJigsawAuthTokenUseCase.Execute(redisId, hackneyToken).Result;

        var dataSource = _dataSourceGateway.GetEntityById(2);

        var jigsawApiId = new SystemId { SystemName = dataSource.Name, Id = $"{firstName}+{lastName}" };

        var response = new SearchResponseObject { SystemIds = new List<SystemId> { jigsawApiId } };

        if (!string.IsNullOrEmpty(authGatewayResponse.ExceptionMessage))
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
            var person = new SearchResult
            {
                Id = result.Id,
                DataSources = new List<string> { dataSource.Name },
                FirstName = result.FirstName.Upcase(),
                SurName = result.LastName.Upcase(),
                DateOfBirth = result.DoB,
                NiNumber = result.NiNumber,
                KnownAddresses = new List<KnownAddress>
                {
                    new() { CurrentAddress = true, FullAddress = result.Address }
                }
            };

            personResults.Add(person);
        }

        response.SearchResponse = new SearchResponse { UngroupedResults = personResults, Total = personResults.Count };


        return response;
    }
}
