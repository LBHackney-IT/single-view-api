using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using ServiceStack.MiniProfiler.Data;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetCouncilTaxAccountsByCustomerNameUseCase : IGetCouncilTaxAccountsByCustomerNameUseCase
{
    private readonly IAcademyGateway _academyGateway;
    private readonly IDataSourceGateway _dataSourceGateway;

    public GetCouncilTaxAccountsByCustomerNameUseCase(IAcademyGateway academyGateway, IDataSourceGateway dataSourceGateway)
    {
        _academyGateway = academyGateway;
        _dataSourceGateway = dataSourceGateway;

    }

    [LogCall]
    public async Task<SearchResponseObject> Execute(string firstName, string lastName, string userToken)
    {
        Console.WriteLine("Making request to Academy Council Tax Use Case");
        var dataSource = _dataSourceGateway.GetEntityById(3);

        var academyApiId = new SystemId() { SystemName = dataSource.Name, Id = $"{firstName}+{lastName}" };

        var response = new SearchResponseObject() { SystemIds = new List<SystemId>() { academyApiId } };

        var accounts = await _academyGateway.GetCouncilTaxAccountsByCustomerName(firstName, lastName, userToken);

        if (accounts.Error != null)
        {
            Console.WriteLine($"Error from gateway: {accounts.Error}");
            academyApiId.Error = accounts.Error;
            return response;
        }

        if (accounts.Customers?.Count == 0)
        {
            Console.WriteLine($"no results found from Academy");
            academyApiId.Error = SystemId.NotFoundMessage;
            return response;
        }

        Console.WriteLine("Converting Academy response into search response....");

        var searchResults = new List<SearchResult>();

        foreach (var account in accounts.Customers)
        {
            var result = new SearchResult()
            {
                Id = account.Id,
                DataSource = dataSource.Name,
                FirstName = account.FirstName,
                SurName = account.LastName,
                DateOfBirth = account?.DateOfBirth,
                NiNumber = account?.NiNumber,
                KnownAddresses = new List<KnownAddress>()
                {
                    new KnownAddress() { CurrentAddress = true, FullAddress = AddressToString(account.FullAddress)  }
                },

            };

            searchResults.Add(result);
        }

        Console.WriteLine($"Search results are {JsonSerializer.Serialize(searchResults)}");

        response.SearchResponse = new SearchResponse()
        {
            SearchResults = searchResults,
            Total = searchResults.Count
        };

        Console.WriteLine($"returning response...");

        return response;
    }

    public string AddressToString(AcademyAddress address)
    {
        return $"{address?.Line1} {address?.Line2} {address?.Line3} {address?.Line4} {address?.Postcode}";
    }
}
