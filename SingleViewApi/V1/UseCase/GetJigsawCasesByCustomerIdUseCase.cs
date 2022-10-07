using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
    private readonly IGetJigsawAuthTokenUseCase _getJigsawAuthTokenUseCase;


    public GetJigsawCasesByCustomerIdUseCase(IJigsawGateway jigsawGateway,
        IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase)
    {
        _jigsawGateway = jigsawGateway;
        _getJigsawAuthTokenUseCase = getJigsawAuthTokenUseCase;
    }

    [LogCall]
    public async Task<CasesResponseObject> Execute(string customerId, string redisId, string hackneyToken)
    {
        var jigsawAuthResponse = _getJigsawAuthTokenUseCase.Execute(redisId, hackneyToken).Result;

        if (!string.IsNullOrEmpty(jigsawAuthResponse.ExceptionMessage))
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

        var currentCase = cases.Cases.FirstOrDefault(x => x.IsCurrent);

        var customerCaseOverview = new JigsawCaseOverviewResponseObject();
        JigsawCasePlacementInformationResponseObject customerAccommodationPlacementList = null;

        var householdCompositionResponse = new JigsawHouseholdCompositionResponseObject();

        if (currentCase != null)
        {
            customerCaseOverview =
                await _jigsawGateway.GetCaseOverviewByCaseId(currentCase.Id.ToString(), jigsawAuthResponse.Token);
            customerAccommodationPlacementList =
                await _jigsawGateway.GetCaseAccommodationPlacementsByCaseId(currentCase.Id.ToString(),
                    jigsawAuthResponse.Token);
            householdCompositionResponse =
                await _jigsawGateway.GetHouseholdCompositionByCaseId(currentCase.Id.ToString(),
                    jigsawAuthResponse.Token);
        }

        if (customerAccommodationPlacementList != null)
            customerAccommodationPlacements.Add(customerAccommodationPlacementList);

        var newCaseOverview = new CaseOverview();
        var householdComposition = new List<JigsawHouseholdMember>();

        if (householdCompositionResponse != null)
            foreach (var householdMember in householdCompositionResponse.People)
            {
                var newMember = new JigsawHouseholdMember
                {
                    DateOfBirth = householdMember.DateOfBirth?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Gender = householdMember.Gender,
                    Name = householdMember.DisplayName,
                    NhsNumber = householdMember.NhsNumber,
                    NiNumber = householdMember.NationalInsuranceNumber
                };

                householdComposition.Add(newMember);
            }


        if (customerCaseOverview != null)
            newCaseOverview = new CaseOverview
            {
                Id = customerCaseOverview.Id.ToString(),
                CurrentDecision = customerCaseOverview.CurrentDecision,
                CurrentFlowchartPosition = customerCaseOverview.CurrentFlowchartPosition,
                HouseholdComposition = householdComposition
            };

        var placementsList = new List<AccommodationPlacementInfo>();

        foreach (var placementList in customerAccommodationPlacements)
        foreach (var placement in placementList.Placements)
        {
            var newPlacement = new AccommodationPlacementInfo
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

        if (currentCase != null)
        {
            var healthAndWellBeingInfo =
                await _jigsawGateway.GetCaseHealthAndWellBeing(currentCase.Id.ToString(), jigsawAuthResponse.Token);

            var additionalFactorsInfo =
                await _jigsawGateway.GetCaseAdditionalFactors(currentCase.Id.ToString(), jigsawAuthResponse.Token);

            var newCaseResponseObject = new CasesResponseObject
            {
                CurrentCase = currentCase,
                CaseOverview = newCaseOverview,
                PlacementInformation = placementsList,
                HealthAndWellBeing = ProcessAdditionalInfo(healthAndWellBeingInfo),
                AdditionalFactors = ProcessAdditionalInfo(additionalFactorsInfo)
            };

            return newCaseResponseObject;
        }

        return null;
    }

    private List<AdditionalInfo> ProcessAdditionalInfo(JigsawCaseAdditionalFactorsResponseObject additionalInformation)
    {
        if (additionalInformation?.QuestionGroups == null) return null;

        var processedInformation = new List<AdditionalInfo>();

        foreach (var questionGroup in additionalInformation.QuestionGroups)
        {
            var information = new List<Information>();

            foreach (var question in questionGroup.Questions)
                information.Add(new Information
                {
                    Question = RemoveAsterisk(question.Label),
                    Answer = RemoveAsterisk(question.GetAnswer(question.SelectedValue))
                });

            processedInformation.Add(new AdditionalInfo { Info = information, Legend = questionGroup.Legend });
        }

        return processedInformation;
    }

    private static string RemoveAsterisk(string data)
    {
        if (string.IsNullOrEmpty(data)) return null;
        return Regex.Replace(data, "[*]", "").Trim();
    }
}
