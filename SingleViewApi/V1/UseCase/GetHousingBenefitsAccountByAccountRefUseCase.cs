using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using Hackney.Shared.Person.Domain;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetHousingBenefitsAccountByAccountRefUseCase : IGetHousingBenefitsAccountByAccountRefUseCase
{
    private readonly IAcademyGateway _academyGateway;
    private readonly IDataSourceGateway _dataSourceGateway;

    public GetHousingBenefitsAccountByAccountRefUseCase(IAcademyGateway academyGateway, IDataSourceGateway dataSourceGateway)
    {
        _academyGateway = academyGateway;
        _dataSourceGateway = dataSourceGateway;
    }

    [LogCall]
    public async Task<CustomerResponseObject> Execute(string accountRef, string userToken)
    {
        var account = await _academyGateway.GetHousingBenefitsAccountByAccountRef(accountRef, userToken);
        var dataSource = _dataSourceGateway.GetEntityById(4);

        var academyCtId = new SystemId() { SystemName = dataSource.Name, Id = accountRef };

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
                Id = accountRef,
                DateOfDeath = account.DateOfBirth,
                NiNo = account.NiNumber,
                DataSource = dataSource,
                FirstName = account.FirstName.Upcase(),
                Surname = account.LastName.Upcase(),
                HousingBenefitsAccount = new HousingBenefitsAccountInfo()
                {
                    ClaimId = account.ClaimId,
                    CheckDigit = account.CheckDigit,
                    PersonReference = account.PersonReference,
                    HouseholdMembers = account.HouseholdMembers,
                    Benefits = account.Benefits,
                    WeeklyHousingBenefitDetails = account.HousingBenefitDetails,
                    HousingBenefitLandlordDetails = account.HousingBenefitLandlordDetails,
                    LastPaymentDetails = account.LastPaymentDetails

                }
            };
        }
        return response;
    }
}
