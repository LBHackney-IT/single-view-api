using System.Collections.Generic;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Boundary;

public class HousingBenefitsAccountInfo
{
    public int ClaimId { get; set; }
#nullable enable
    public string? CheckDigit { get; set; }
    public string? PersonReference { get; set; }
    public List<HouseholdMember>? HouseholdMembers { get; set; }
    public List<Benefits>? Benefits { get; set; }
    public HbInfo? WeeklyHousingBenefitDetails { get; set; }
    public HousingBenefitLandlordDetails? HousingBenefitLandlordDetails { get; set; }
    public PaymentDetails? LastPaymentDetails { get; set; }
#nullable disable
}
