using System;
using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response;

public class CouncilTaxSearchResponseObject
{
    public List<CouncilTaxSearchResponse> Customers { get; set; }

#nullable enable
    public string? Error { get; set; }

#nullable disable
}

public class CouncilTaxSearchResponse
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
#nullable enable
    public DateTime? DateOfBirth { get; set; }
    public string? NiNumber { get; set; }
#nullable disable
    public Address FullAddress { get; set; }
    public string PostCode { get; set; }
}


