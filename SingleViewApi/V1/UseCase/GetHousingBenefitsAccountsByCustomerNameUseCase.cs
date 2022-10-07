using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetHousingBenefitsAccountsByCustomerNameUseCase : IGetHousingBenefitsAccountsByCustomerNameUseCase
{
    private readonly IAcademyGateway _academyGateway;
    private readonly IDataSourceGateway _dataSourceGateway;

    public GetHousingBenefitsAccountsByCustomerNameUseCase(IAcademyGateway academyGateway,
        IDataSourceGateway dataSourceGateway)
    {
        _academyGateway = academyGateway;
        _dataSourceGateway = dataSourceGateway;
    }

    [LogCall]
    public async Task<SearchResponseObject> Execute(string firstName, string lastName, string userToken)
    {
        var dataSource = _dataSourceGateway.GetEntityById(4);
        var academyApiId = new SystemId { SystemName = dataSource.Name, Id = $"{firstName}+{lastName}" };
        var response = new SearchResponseObject { SystemIds = new List<SystemId> { academyApiId } };
        var accounts = await _academyGateway.GetHousingBenefitsAccountsByCustomerName(firstName, lastName, userToken);

        if (accounts?.Customers == null || accounts.Customers.Count == 0)
        {
            academyApiId.Error = SystemId.NotFoundMessage;
            return response;
        }

        if (accounts.Error != null)
        {
            Console.WriteLine($"Error from Academy Housing Benefits Use Case: {accounts.Error}");
            academyApiId.Error = accounts.Error;
            return response;
        }

        var searchResults = new List<SearchResult>();

        foreach (var account in accounts.Customers)
        {
            var result = new SearchResult
            {
                Id = account.Id,
                DataSources = new List<string> { dataSource.Name },
                FirstName = account.FirstName.Upcase(),
                SurName = account.LastName,
                DateOfBirth = account?.DateOfBirth,
                NiNumber = account?.NiNumber,
                KnownAddresses = new List<KnownAddress>
                {
                    new() { CurrentAddress = true, FullAddress = AddressToString(account.FullAddress) }
                }
            };
            searchResults.Add(result);
        }

        response.SearchResponse = new SearchResponse { UngroupedResults = searchResults, Total = searchResults.Count };
        return response;
    }

    public string AddressToString(Address address)
    {
        return $"{address?.Line1} {address?.Line2} {address?.Line3} {address?.Line4} {address?.Postcode}";
    }
}
