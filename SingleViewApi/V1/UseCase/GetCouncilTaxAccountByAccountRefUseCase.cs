using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using Hackney.Shared.Person.Domain;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetCouncilTaxAccountByIdUseCase : IGetCouncilTaxAccountByAccountRefUseCase
{
    private readonly IAcademyGateway _academyGateway;
    private readonly IDataSourceGateway _dataSourceGateway;

    public GetCouncilTaxAccountByIdUseCase(IAcademyGateway academyGateway, IDataSourceGateway dataSourceGateway)
    {
        _academyGateway = academyGateway;
        _dataSourceGateway = dataSourceGateway;
    }

    [LogCall]
    public async Task<CustomerResponseObject> Execute(string accountRef, string userToken)
    {
        var account = await _academyGateway.GetCouncilTaxAccountByAccountRef(accountRef, userToken);
        var dataSource = _dataSourceGateway.GetEntityById(3);

        var academyCtId = new SystemId() { SystemName = dataSource.Name, Id = account.AccountReference.ToString() };

        var response = new CustomerResponseObject()
        {
            SystemIds = new List<SystemId>() { academyCtId }
        };

        if (account == null)
        {
            academyCtId.Error = SystemId.NotFoundMessage;
        }
        else
        {
            response.Customer = new Customer()
            {
                Id = account.AccountReference.ToString(),
                DataSource = dataSource,
                FirstName = account.FirstName.Upcase(),
                Surname = account.LastName.Upcase(),
                CouncilTaxAccount = new CouncilTaxAccountInfo()
                {
                    AccountBalance = account.AccountBalance,
                    AccountCheckDigit = account.AccountCheckDigit,
                    AccountReference = account.AccountReference,
                    ForwardingAddress = account.ForwardingAddress,
                    PropertyAddress = account.PropertyAddress,
                    PaymentMethod = account.PaymentMethod
                }
            };
        }
        return response;
    }
}
