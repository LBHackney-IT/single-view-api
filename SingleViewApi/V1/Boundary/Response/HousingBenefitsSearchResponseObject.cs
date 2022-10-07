using System;
using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response;

public class HousingBenefitsSearchResponseObject
{
    public List<HousingBenefitsSearchResponse> Customers { get; set; }

#nullable enable
    public string? Error { get; set; }

#nullable disable
}

public class HousingBenefitsSearchResponse
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
#nullable enable
    public DateTime? DateOfBirth { get; set; }
    public string? NiNumber { get; set; }
#nullable disable
    public Address FullAddress { get; set; }
}
