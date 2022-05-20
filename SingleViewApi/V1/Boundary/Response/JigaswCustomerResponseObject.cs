using System;

namespace SingleViewApi.V1.Boundary.Response;
    public class Address
    {
        public int Id { get; set; }
        public string Aapartment { get; set; }
        public object RoomNumber { get; set; }
        public string HouseName { get; set; }
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public object Locality { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public object Latitude { get; set; }
        public object Longitude { get; set; }
        public object Freetext { get; set; }
    }

    public class CustomerCreationOverrides
    {
        public bool IsNiNumberMandatory { get; set; }
        public bool ShowNhsNumber { get; set; }
        public bool ShowPregnancyDueDate { get; set; }
        public bool ShowMiddleName { get; set; }
        public bool ShowPreferredName { get; set; }
        public bool ShowOtherName { get; set; }
        public bool ShowSupportWorkerCollection { get; set; }
        public int MinCustomerAge { get; set; }
        public bool CollectMoveInDate { get; set; }
        public bool CollectAccommodationType { get; set; }
        public bool CollectHousingCircumstance { get; set; }
        public bool CollectWasThisSettledAccommodation { get; set; }
        public bool CollectLandlordAccommodationProvider { get; set; }
        public bool CollectNiNumber { get; set; }
        public bool CollectCorrespondenceAddress { get; set; }
        public bool CollectPreferredLanguage { get; set; }
        public bool CollectOkToCall { get; set; }
        public bool CollectPreferredContactMethod { get; set; }
    }

    public class PersonInfo
    {
        public int Id { get; set; }
        public bool HasUserAccount { get; set; }
        public int PersonTitleId { get; set; }
        public object RelationshipId { get; set; }
        public int GenderId { get; set; }
        public string PersonTitle { get; set; }
        public string FirstName { get; set; }
        public object MiddleName { get; set; }
        public object OtherName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public object PreferredName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string NationalInsuranceNumber { get; set; }
        public object NhsNumber { get; set; }
        public object PregnancyDueDate { get; set; }
        public string AddressString { get; set; }
        public Address Address { get; set; }
        public object MoveInDate { get; set; }
        public object AccommodationTypeId { get; set; }
        public object HousingCircumstanceId { get; set; }
        public bool IsSettled { get; set; }
        public object AccommodationProvider { get; set; }
        public object CorrespondenceAddress { get; set; }
        public object CorrespondenceAddressString { get; set; }
        public object HomePhoneNumber { get; set; }
        public bool OkToContactOnHomePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public bool OkToContactOnMobilePhoneNumber { get; set; }
        public object WorkPhoneNumber { get; set; }
        public bool OkToContactOnWorkPhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool OkToContactOnEmail { get; set; }
        public object SupportWorker { get; set; }
        public object SupportWorkerId { get; set; }
        public string Gender { get; set; }
        public object HouseholdMemberType { get; set; }
        public object ContactMethodId { get; set; }
        public object LanguageId { get; set; }
        public object PreferredLanguage { get; set; }
        public object ContactMethod { get; set; }
        public object LocalAuthorityId { get; set; }
        public object OrganisationInternalReference { get; set; }
    }

    public class JigsawCustomerResponseObject
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public bool HasAccount { get; set; }
        public bool HasEhrModule { get; set; }
        public bool HasPrahModule { get; set; }
        public bool HasAraModule { get; set; }
        public bool HasRiseModule { get; set; }
        public bool HasCrmModule { get; set; }
        public PersonInfo PersonInfo { get; set; }
        public int NotesCount { get; set; }
        public object Referrer { get; set; }
        public int ReferralSourceId { get; set; }
        public object ReferralSources { get; set; }
        public bool CanSendSms { get; set; }
        public object OrganisationInternalReference { get; set; }
        public object LastAccessedDashboard { get; set; }
        public int SensitiveNotesCount { get; set; }
        public int PinnedNotesCount { get; set; }
        public CustomerCreationOverrides CustomerCreationOverrides { get; set; }
    }
