using System;
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
    private IGetJigsawAuthTokenUseCase _getJigsawAuthTokenUseCase;


    public GetJigsawCasesByCustomerIdUseCase(IJigsawGateway jigsawGateway, IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase)
    {
        _jigsawGateway = jigsawGateway;
        _getJigsawAuthTokenUseCase = getJigsawAuthTokenUseCase;
    }

    [LogCall]
    public async Task<dynamic> Execute(string customerId, string redisId, string hackneyToken)
    {
        var jigsawAuthResponse = _getJigsawAuthTokenUseCase.Execute(redisId, hackneyToken).Result;

        if (!String.IsNullOrEmpty(jigsawAuthResponse.ExceptionMessage))
        {
            Console.WriteLine($"Error getting Jigsaw token for CustomerById: {jigsawAuthResponse.ExceptionMessage}");
            return null;
        }
        var cases = await _jigsawGateway.GetCasesByCustomerId(customerId, jigsawAuthResponse.Token);

        var customerCaseOverviews = new List<dynamic>();
        var customerAccommodationPlacements = new List<dynamic>();

        foreach (var customerCase in cases.Cases)
        {
            var customerCaseOverview = await _jigsawGateway.GetCaseOverviewByCaseId(customerCase.Id.ToString(), jigsawAuthResponse.Token);
            var customerAccommodationPlacement =
                await _jigsawGateway.GetCaseAccommodationPlacementsByCaseId(customerCase.Id.ToString(), jigsawAuthResponse.Token);

            customerCaseOverviews.Add(customerCaseOverview);
            customerAccommodationPlacements.Add(customerAccommodationPlacement);
        }

        //what do we want our response object to look like?
        return customerCaseOverviews.Append(customerAccommodationPlacements);

    }
}
