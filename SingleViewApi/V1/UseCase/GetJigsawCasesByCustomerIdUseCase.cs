using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using ServiceStack;
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

        if (cases == null)
        {
            Console.WriteLine($"No Cases found for id {customerId}");
            return null;
        }

        var customerAccommodationPlacements = new List<JigsawCasePlacementInformationResponseObject>();

        var currentCase = cases.Cases.First(x => x.IsCurrent);

        var customerCaseOverview = await _jigsawGateway.GetCaseOverviewByCaseId(currentCase.Id.ToString(), jigsawAuthResponse.Token);
        var customerAccommodationPlacementList =
            await _jigsawGateway.GetCaseAccommodationPlacementsByCaseId(currentCase.Id.ToString(), jigsawAuthResponse.Token);

        if (customerAccommodationPlacementList != null)
        {
            customerAccommodationPlacements.Add(customerAccommodationPlacementList);
        }

        var newCaseOverview = new CaseOverview();

        if (customerCaseOverview != null)
        {
            newCaseOverview = new CaseOverview()
            {
                Id = customerCaseOverview.Id.ToString(),
                CurrentDecision = customerCaseOverview.CurrentDecision,
                CurrentFlowchartPosition = customerCaseOverview.CurrentFlowchartPosition,
                HouseHoldComposition = customerCaseOverview.HouseholdComposition
            };

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
                    PlacementDutyFullName = placement.PlacementDutyFullName,
                    PlacementType = placement.PlacementType,
                    Usage = placement.Usage
                };

                placementsList.Add(newPlacement);
            }
        }

        var newCaseResponseObject = new CasesResponseObject()
        {
            CurrentCase = currentCase,
            CaseOverview = newCaseOverview,
            PlacementInformation = placementsList
        };

        return newCaseResponseObject;

    }


}
