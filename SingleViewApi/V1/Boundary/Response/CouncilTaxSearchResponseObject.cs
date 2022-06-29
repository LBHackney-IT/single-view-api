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
    public AcademyAddress FullAddress { get; set; }
    public string PostCode { get; set; }
}

public class AcademyAddress
{
    public string Line1 { get; set; }
    public string Line2 { get; set; }
    public string Line3 { get; set; }
    public string Line4 { get; set; }
    public string Postcode { get; set; }
}
