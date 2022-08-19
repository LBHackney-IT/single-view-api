using System;
using System.Collections.Generic;
using Hackney.Shared.ContactDetail.Domain;
using Hackney.Shared.Person.Domain;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Boundary.Response
{
    public class MergedCustomerResponseObject
    {
#nullable enable
        public MergedCustomer? Customer { get; set; }
#nullable disable
        public List<SystemId> SystemIds { get; set; }

    }

    public class CustomerContactDetails
    {
        public string DataSourceName { get; set; }
        public bool IsActive { get; set; }
        public string SourceServiceArea { get; set; }
        public string ContactType { get; set; }
        public string SubType { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public AddressExtended AddressExtended { get; set; }
    }
    public class MergedCustomer
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string PreferredTitle { get; set; }
        public string PreferredFirstName { get; set; }
        public string PreferredMiddleName { get; set; }
        public string PreferredSurname { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string PlaceOfBirth { get; set; }
        public List<KnownAddress> KnownAddresses { get; set; }
        public List<CustomerContactDetails> AllContactDetails { get; set; }

        public List<String> PersonTypes { get; set; }
#nullable enable
        public CouncilTaxAccountInfo? CouncilTaxAccount { get; set; }
        public HousingBenefitsAccountInfo? HousingBenefitsAccount { get; set; }
        public List<CautionaryAlert>? CautionaryAlerts { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? NiNo { get; set; }
        public string? NhsNumber { get; set; }
        public string? PregnancyDueDate { get; set; }
        public string? AccommodationType { get; set; }
        public string? HousingCircumstance { get; set; }
        public bool? IsSettled { get; set; }
        public SupportWorker? SupportWorker { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public bool? IsAMinor { get; set; }
        public EqualityInformationResponseObject? EqualityInformation { get; set; }
#nullable disable
    }
}
