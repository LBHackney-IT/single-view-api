using System;
using System.Collections.Generic;
using Hackney.Shared.Person.Domain;

namespace SingleViewApi.V1.Boundary.Response;

public class HousingBenefitsRecordResponseObject
{
    public int ClaimId { get; set; }

#nullable enable

    public string? CheckDigit { get; set; }

    public string? PersonReference { get; set; }

    public Title? Title { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? NiNumber { get; set; }

    public Address? FullAddress { get; set; }

    public string? PostCode { get; set; }

    public List<HouseholdMember>? HouseholdMembers { get; set; }

    public List<Benefits>? Benefits { get; set; }

    public HbInfo? HousingBenefitDetails { get; set; }

    public HousingBenefitLandlordDetails? HousingBenefitLandlordDetails { get; set; }

    public PaymentDetails? LastPaymentDetails { get; set; }

#nullable disable
}

public class Benefits
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string Period { get; set; }
    public int Frequency { get; set; }
}

public class HbInfo
{
    public string HousingBenefitPayee { get; set; }
    public decimal WeeklyHousingBenefit { get; set; }
}

public class HousingBenefitLandlordDetails
{
    public int ClaimId { get; set; }
    public string Name { get; set; }
    public string Addr1 { get; set; }
    public string Addr2 { get; set; }
    public string Addr3 { get; set; }
    public string Addr4 { get; set; }
    public string Postcode { get; set; }
    public int CreditorId { get; set; }
}

public class HouseholdMember
{
    public string Title { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }
}

public class PaymentDetails
{
    public int ClaimId { get; set; }
    public DateTime PostingDate { get; set; }
    public decimal PaymentAmount { get; set; }
}


