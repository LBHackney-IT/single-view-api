using System;
using System.Collections.Generic;
using Hackney.Shared.Tenure.Domain;

namespace SingleViewApi.V1.Boundary;

public class KnownAddress
{
    public string FullAddress { get; set; }
    public Guid Id { get; set; }
    public bool CurrentAddress { get; set; }
    public string DataSourceName { get; set; }
#nullable enable
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public List<HouseholdMembers>? HouseholdMembers { get; set; }
    public List<LegacyReference>? LegacyReferences { get; set; }
#nullable disable
}
