using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Boundary.Response;


public class HouseholdCompositionAddress
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("apartment")]
    public string Apartment { get; set; }

#nullable enable

    [JsonProperty("roomNumber")]
    public int? RoomNumber { get; set; }

    [JsonProperty("latitude")]
    public string? Latitude { get; set; }

    [JsonProperty("longitude")]
    public string? Longitude { get; set; }

    [JsonProperty("freetext")]
    public string? Freetext { get; set; }

    [JsonProperty("locality")]
    public string? Locality { get; set; }

#nullable disable

    [JsonProperty("houseName")]
    public string HouseName { get; set; }

    [JsonProperty("houseNumber")]
    public string HouseNumber { get; set; }

    [JsonProperty("street")]
    public string Street { get; set; }

    [JsonProperty("town")]
    public string Town { get; set; }

    [JsonProperty("county")]
    public string County { get; set; }

    [JsonProperty("postcode")]
    public string Postcode { get; set; }

    [JsonProperty("isCurrent")]
    public bool IsCurrent { get; set; }
}

public class JigsawHouseholdPerson
{
    [JsonProperty("casePersonId")]
    public int CasePersonId { get; set; }

    [JsonProperty("referralPersonId")]
    public int? ReferralPersonId { get; set; }

    [JsonProperty("isLead")]
    public bool IsLead { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("personTitleId")]
    public int PersonTitleId { get; set; }

    [JsonProperty("relationshipId")]
    public int? RelationshipId { get; set; }

    [JsonProperty("genderId")]
    public int GenderId { get; set; }

    [JsonProperty("customerId")]
    public int CustomerId { get; set; }

    [JsonProperty("personTitle")]
    public string PersonTitle { get; set; }

    [JsonProperty("firstName")]
    public string FirstName { get; set; }

#nullable enable

    [JsonProperty("middleName")]
    public string? MiddleName { get; set; }

    [JsonProperty("otherName")]
    public string? OtherName { get; set; }

    [JsonProperty("nhsNumber")]
    public string? NhsNumber { get; set; }

    [JsonProperty("pregnancyDueDate")]
    public string? PregnancyDueDate { get; set; }

    [JsonProperty("housingCircumstance")]
    public string? HousingCircumstance { get; set; }

    [JsonProperty("accommodationDescription")]
    public string? AccommodationDescription { get; set; }

    [JsonProperty("accommodationProvider")]
    public string? AccommodationProvider { get; set; }

    [JsonProperty("correspondenceAddress")]
    public string? CorrespondenceAddress { get; set; }

    [JsonProperty("correspondenceAddressString")]
    public string? CorrespondenceAddressString { get; set; }

    [JsonProperty("homePhoneNumber")]
    public string? HomePhoneNumber { get; set; }

    [JsonProperty("supportWorkerId")]
    public int? SupportWorkerId { get; set; }

    [JsonProperty("contactMethodId")]
    public int? ContactMethodId { get; set; }

    [JsonProperty("contactMethod")]
    public int? ContactMethod { get; set; }

    [JsonProperty("customerReference")]
    public string? CustomerReference { get; set; }

    [JsonProperty("workPhoneNumber")]
    public string? WorkPhoneNumber { get; set; }

    [JsonProperty("moveInDate")]
    public DateTime? MoveInDate { get; set; }

    [JsonProperty("lastName")]
    public string? LastName { get; set; }

    [JsonProperty("displayName")]
    public string? DisplayName { get; set; }

    [JsonProperty("dateOfBirth")]
    public DateTime? DateOfBirth { get; set; }

    [JsonProperty("nationalInsuranceNumber")]
    public string? NationalInsuranceNumber { get; set; }

    [JsonProperty("addressString")]
    public string? AddressString { get; set; }

    [JsonProperty("address")]
    public HouseholdCompositionAddress? Address { get; set; }

    [JsonProperty("accommodationTypeId")]
    public int? AccommodationTypeId { get; set; }

    [JsonProperty("housingCircumstanceId")]
    public int? HousingCircumstanceId { get; set; }

    [JsonProperty("isSettled")]
    public bool? IsSettled { get; set; }

    [JsonProperty("okToContactOnHomePhoneNumber")]
    public bool? OkToContactOnHomePhoneNumber { get; set; }

    [JsonProperty("mobilePhoneNumber")]
    public string? MobilePhoneNumber { get; set; }

    [JsonProperty("okToContactOnMobilePhoneNumber")]
    public bool? OkToContactOnMobilePhoneNumber { get; set; }

    [JsonProperty("okToContactOnWorkPhoneNumber")]
    public bool? OkToContactOnWorkPhoneNumber { get; set; }

    [JsonProperty("emailAddress")]
    public string? EmailAddress { get; set; }

    [JsonProperty("okToContactOnEmail")]
    public bool? OkToContactOnEmail { get; set; }

    [JsonProperty("contactDetailsId")]
    public int? ContactDetailsId { get; set; }

    [JsonProperty("supportWorker")]
    public JigsawSupportWorker? SupportWorker { get; set; }

    [JsonProperty("gender")]
    public string? Gender { get; set; }

    [JsonProperty("householdMemberType")]
    public int? HouseholdMemberType { get; set; }

    [JsonProperty("preferredLanguage")]
    public string? PreferredLanguage { get; set; }

#nullable disable
}

public class JigsawHouseholdCompositionResponseObject
{
    [JsonProperty("people")]
    public List<JigsawHouseholdPerson> People { get; set; }

}

public class JigsawSupportWorker
{
    [JsonProperty("id")]
    public int Id { get; set; }

#nullable enable
    [JsonProperty("firstName")]
    public string? FirstName { get; set; }

    [JsonProperty("lastName")]
    public string? LastName { get; set; }

    [JsonProperty("fullName")]
    public string? FullName { get; set; }

    [JsonProperty("jobTitle")]
    public string? JobTitle { get; set; }

    [JsonProperty("agency")]
    public string? Agency { get; set; }

    [JsonProperty("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonProperty("emailAddress")]
    public string? EmailAddress { get; set; }
#nullable disable
}


