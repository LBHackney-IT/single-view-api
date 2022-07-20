using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
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
    public async Task<CasesResponseObject> Execute(string customerId, string redisId, string hackneyToken)
    {
        var jigsawAuthResponse = _getJigsawAuthTokenUseCase.Execute(redisId, hackneyToken).Result;

        if (!String.IsNullOrEmpty(jigsawAuthResponse.ExceptionMessage))
        {
            Console.WriteLine($"Error getting Jigsaw token for CustomerById: {jigsawAuthResponse.ExceptionMessage}");
            return null;
        }
        var cases = await _jigsawGateway.GetCasesByCustomerId(customerId, jigsawAuthResponse.Token);

        var customerCaseOverviews = new List<JigsawCaseOverviewResponseObject>();
        var customerAccommodationPlacements = new List<JigsawCasePlacementInformationResponseObject>();

        foreach (var customerCase in cases.Cases)
        {
            var customerCaseOverview = await _jigsawGateway.GetCaseOverviewByCaseId(customerCase.Id.ToString(), jigsawAuthResponse.Token);
            var customerAccommodationPlacementList =
                await _jigsawGateway.GetCaseAccommodationPlacementsByCaseId(customerCase.Id.ToString(), jigsawAuthResponse.Token);

            if (customerCaseOverview != null)
            {
                customerCaseOverviews.Add(customerCaseOverview);
            }

            if (customerAccommodationPlacementList != null)
            {
                customerAccommodationPlacements.Add(customerAccommodationPlacementList);
            }

        }

        var caseOverviews = new List<CaseOverview>();

        foreach (var caseOverview in customerCaseOverviews)
        {
            var newCaseOverview = new CaseOverview()
            {
                Id = caseOverview.Id.ToString(),
                CurrentDecision = caseOverview.CurrentDecision,
                CurrentFlowchartPosition = caseOverview.CurrentFlowchartPosition,
                HouseHoldComposition = caseOverview.HouseholdComposition
            };

            caseOverviews.Add(newCaseOverview);
        }

        var placementsList = new List<AccommodationPlacementInfo>();

        foreach (var placementList in customerAccommodationPlacements)
        {
            foreach (var placement in placementList.Placements)
            {
                var newPlacement = new AccommodationPlacementInfo()
                {
                    DclgClassificationType = placement.DclgClassificationType,
                    FullAddressDetails = placement.FullAddressDetails,
                    PlacementDuty = placement.PlacementDuty,
                    PLacementDutyFullName = placement.PlacementDutyFullName,
                    PlacementType = placement.PlacementType,
                    Usage = placement.Usage
                };

                placementsList.Add(newPlacement);
            }
        }

        var newCaseResponseObject = new CasesResponseObject()
        {
            Cases = cases.Cases,
            CaseOverviews = caseOverviews,
            PlacementInformation = placementsList
        };

        return newCaseResponseObject;

    }


}
