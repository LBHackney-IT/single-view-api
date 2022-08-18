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

        [JsonProperty("roomNumber")]
        public object RoomNumber { get; set; }

        [JsonProperty("houseName")]
        public string HouseName { get; set; }

        [JsonProperty("houseNumber")]
        public string HouseNumber { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("locality")]
        public object Locality { get; set; }

        [JsonProperty("town")]
        public string Town { get; set; }

        [JsonProperty("county")]
        public string County { get; set; }

        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        [JsonProperty("latitude")]
        public object Latitude { get; set; }

        [JsonProperty("longitude")]
        public object Longitude { get; set; }

        [JsonProperty("freetext")]
        public object Freetext { get; set; }

        [JsonProperty("isCurrent")]
        public bool IsCurrent { get; set; }
    }

    public class Person
    {
        [JsonProperty("casePersonId")]
        public int CasePersonId { get; set; }

        [JsonProperty("referralPersonId")]
        public object ReferralPersonId { get; set; }

        [JsonProperty("isLead")]
        public bool IsLead { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("personTitleId")]
        public int PersonTitleId { get; set; }

        [JsonProperty("relationshipId")]
        public object RelationshipId { get; set; }

        [JsonProperty("genderId")]
        public int GenderId { get; set; }

        [JsonProperty("customerId")]
        public int CustomerId { get; set; }

        [JsonProperty("personTitle")]
        public string PersonTitle { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("middleName")]
        public object MiddleName { get; set; }

        [JsonProperty("otherName")]
        public object OtherName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("nationalInsuranceNumber")]
        public string NationalInsuranceNumber { get; set; }

        [JsonProperty("nhsNumber")]
        public object NhsNumber { get; set; }

        [JsonProperty("pregnancyDueDate")]
        public object PregnancyDueDate { get; set; }

        [JsonProperty("addressString")]
        public string AddressString { get; set; }

        [JsonProperty("address")]
        public HouseholdCompositionAddress Address { get; set; }

        [JsonProperty("moveInDate")]
        public DateTime MoveInDate { get; set; }

        [JsonProperty("accommodationTypeId")]
        public int AccommodationTypeId { get; set; }

        [JsonProperty("housingCircumstanceId")]
        public int HousingCircumstanceId { get; set; }

        [JsonProperty("isSettled")]
        public bool IsSettled { get; set; }

        [JsonProperty("housingCircumstance")]
        public object HousingCircumstance { get; set; }

        [JsonProperty("accommodationDescription")]
        public object AccommodationDescription { get; set; }

        [JsonProperty("accommodationProvider")]
        public object AccommodationProvider { get; set; }

        [JsonProperty("correspondenceAddress")]
        public object CorrespondenceAddress { get; set; }

        [JsonProperty("correspondenceAddressString")]
        public object CorrespondenceAddressString { get; set; }

        [JsonProperty("homePhoneNumber")]
        public object HomePhoneNumber { get; set; }

        [JsonProperty("okToContactOnHomePhoneNumber")]
        public bool OkToContactOnHomePhoneNumber { get; set; }

        [JsonProperty("mobilePhoneNumber")]
        public string MobilePhoneNumber { get; set; }

        [JsonProperty("okToContactOnMobilePhoneNumber")]
        public bool OkToContactOnMobilePhoneNumber { get; set; }

        [JsonProperty("workPhoneNumber")]
        public object WorkPhoneNumber { get; set; }

        [JsonProperty("okToContactOnWorkPhoneNumber")]
        public bool OkToContactOnWorkPhoneNumber { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("okToContactOnEmail")]
        public bool OkToContactOnEmail { get; set; }

        [JsonProperty("contactDetailsId")]
        public int ContactDetailsId { get; set; }

        [JsonProperty("supportWorker")]
        public JigsawSupportWorker SupportWorker { get; set; }

        [JsonProperty("supportWorkerId")]
        public object SupportWorkerId { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("householdMemberType")]
        public int HouseholdMemberType { get; set; }

        [JsonProperty("contactMethodId")]
        public object ContactMethodId { get; set; }

        [JsonProperty("contactMethod")]
        public object ContactMethod { get; set; }

        [JsonProperty("preferredLanguage")]
        public string PreferredLanguage { get; set; }

        [JsonProperty("customerReference")]
        public object CustomerReference { get; set; }
    }

    public class JigsawHouseholdCompositionResponseObject
    {
        [JsonProperty("people")]
        public List<Person> People { get; set; }

    }

    public class JigsawSupportWorker
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("firstName")]
        public object FirstName { get; set; }

        [JsonProperty("lastName")]
        public object LastName { get; set; }

        [JsonProperty("fullName")]
        public object FullName { get; set; }

        [JsonProperty("jobTitle")]
        public object JobTitle { get; set; }

        [JsonProperty("agency")]
        public object Agency { get; set; }

        [JsonProperty("phoneNumber")]
        public object PhoneNumber { get; set; }

        [JsonProperty("emailAddress")]
        public object EmailAddress { get; set; }
    }


