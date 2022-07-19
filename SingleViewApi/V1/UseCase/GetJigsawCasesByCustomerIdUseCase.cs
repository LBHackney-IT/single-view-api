using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetJigsawCasesByCustomerIdUseCase : IGetJigsawCasesByCustomerIdUseCase
{
    private readonly IJigsawGateway _jigsawGateway;

    public GetJigsawCasesByCustomerIdUseCase(IJigsawGateway jigsawGateway)
    {
        _jigsawGateway = jigsawGateway;
    }

    [LogCall]
    public async Task<dynamic> Execute(string customerId, string bearerToken)
    {
        var cases = await _jigsawGateway.GetCasesByCustomerId(customerId, bearerToken);

        var customerCaseOverviews = new List<dynamic>();
        var customerAccommodationPlacements = new List<dynamic>();

        foreach (var customerCase in cases.Cases)
        {
            var customerCaseOverview = await _jigsawGateway.GetCaseOverviewByCaseId(customerCase.Id.ToString(), bearerToken);
            var customerAccommodationPlacement =
                await _jigsawGateway.GetCaseAccommodationPlacementsByCaseId(customerCase.Id.ToString(), bearerToken);

            customerCaseOverviews.Add(customerCaseOverview);
            customerAccommodationPlacements.Add(customerAccommodationPlacement);
        }

        //what do we want our response object to look like?
        return customerCaseOverviews.Append(customerAccommodationPlacements);

    }
}
