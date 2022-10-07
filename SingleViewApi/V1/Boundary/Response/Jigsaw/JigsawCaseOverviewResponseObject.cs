using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Boundary.Response;



public class JigsawCaseOverviewResponseObject
{
    [JsonProperty("id")]
    public int Id { get; set; }



    [JsonProperty("personInfo")]
    public JigsawPersonInfo PersonInfo { get; set; }

    [JsonProperty("customerId")]
    public int CustomerId { get; set; }

#nullable enable

    [JsonProperty("organisationInternalReference")]
    public string? OrganisationInternalReference { get; set; }

    [JsonProperty("referralId")]
    public int? ReferralId { get; set; }

    [JsonProperty("assignedTo")]
    public string? AssignedTo { get; set; }

    [JsonProperty("notesCount")]
    public int? NotesCount { get; set; }

    [JsonProperty("meetingsCount")]
    public int? MeetingsCount { get; set; }

    [JsonProperty("actionsCount")]
    public int? ActionsCount { get; set; }

    [JsonProperty("actionsCompletedCount")]
    public int? ActionsCompletedCount { get; set; }

    [JsonProperty("phpActionsCustomerCount")]
    public int? PhpActionsCustomerCount { get; set; }

    [JsonProperty("phpActionsLACount")]
    public int? PhpActionsLaCount { get; set; }

    [JsonProperty("phpActionsThirdPartyCount")]
    public int? PhpActionsThirdPartyCount { get; set; }

    [JsonProperty("phpActionsCompleteLACount")]
    public int? PhpActionsCompleteLaCount { get; set; }

    [JsonProperty("phpActionsCompleteCustomerCount")]
    public int? PhpActionsCompleteCustomerCount { get; set; }

    [JsonProperty("phpActionsCompleteThirdPartyCount")]
    public int? PhpActionsCompleteThirdPartyCount { get; set; }

    [JsonProperty("requiredDocumentsCount")]
    public int? RequiredDocumentsCount { get; set; }

    [JsonProperty("requiredDocumentsSuppliedCount")]
    public int? RequiredDocumentsSuppliedCount { get; set; }

    [JsonProperty("documentsUploadedCount")]
    public int? DocumentsUploadedCount { get; set; }

    [JsonProperty("officerIdAssignedTo")]
    public int? OfficerIdAssignedTo { get; set; }

    [JsonProperty("currentFlowchartPosition")]
    public string? CurrentFlowchartPosition { get; set; }

    [JsonProperty("currentDecision")]
    public string? CurrentDecision { get; set; }

    [JsonProperty("hasBeenInApproach")]
    public bool? HasBeenInApproach { get; set; }

    [JsonProperty("hasBeenInApplicationTriggered")]
    public bool? HasBeenInApplicationTriggered { get; set; }

    [JsonProperty("hasBeenInPrevention")]
    public bool? HasBeenInPrevention { get; set; }

    [JsonProperty("hasBeenInRelief")]
    public bool? HasBeenInRelief { get; set; }

    [JsonProperty("hasBeenInReliefPendingOutcome")]
    public bool? HasBeenInReliefPendingOutcome { get; set; }

    [JsonProperty("hasBeenInMainDutyDecisionPending")]
    public bool? HasBeenInMainDutyDecisionPending { get; set; }

    [JsonProperty("hasBeenInMainDutyNotOwed")]
    public bool? HasBeenInMainDutyNotOwed { get; set; }

    [JsonProperty("hasBeenInMainDutyAccepted")]
    public bool? HasBeenInMainDutyAccepted { get; set; }

    [JsonProperty("hasBeenClosed")]
    public bool? HasBeenClosed { get; set; }

    [JsonProperty("daysRemaining")]
    public int? DaysRemaining { get; set; }

    [JsonProperty("householdComposition")]
    public string? HouseholdComposition { get; set; }

    [JsonProperty("localAuthorityId")]
    public int? LocalAuthorityId { get; set; }

    [JsonProperty("caseReviews")]
    public List<dynamic>? CaseReviews { get; set; }

    [JsonProperty("hideDeleteButton")]
    public bool? HideDeleteButton { get; set; }

    [JsonProperty("isV2LegacyCase")]
    public bool? IsV2LegacyCase { get; set; }

    [JsonProperty("isLegacyCase")]
    public bool? IsLegacyCase { get; set; }

    [JsonProperty("isLegacyDeleted")]
    public bool? IsLegacyDeleted { get; set; }


    [JsonProperty("whenAppliedOrApproached")]
    public DateTime? WhenAppliedOrApproached { get; set; }

    [JsonProperty("appliedOrApproachedMaxDate")]
    public DateTime? AppliedOrApproachedMaxDate { get; set; }

    [JsonProperty("sensitiveNotesCount")]
    public int? SensitiveNotesCount { get; set; }

    [JsonProperty("pinnedNotesCount")]
    public int? PinnedNotesCount { get; set; }

#nullable disable


}

public class JigsawPersonInfo
{
    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

#nullable enable
    [JsonProperty("otherName")]
    public string? OtherName { get; set; }
    [JsonProperty("nhsNumber")]
    public string? NhsNumber { get; set; }
#nullable disable

    [JsonProperty("gender")]
    public string Gender { get; set; }

    [JsonProperty("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [JsonProperty("nationalInsuranceNumber")]
    public string NationalInsuranceNumber { get; set; }


}


