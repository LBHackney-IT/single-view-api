using System;
using System.Collections.Generic;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Boundary;

public class CasesResponseObject
{
#nullable enable
    public Case? CurrentCase { get; set; }
    public List<AdditionalInfo>? AdditionalFactors { get; set; }

    public List<AdditionalInfo>? HealthAndWellBeing { get; set; }
#nullable disable
    public List<AccommodationPlacementInfo> PlacementInformation { get; set; }

    public CaseOverview CaseOverview { get; set; }
}




public class CaseOverview
{
    public string Id { get; set; }
    public string CurrentFlowchartPosition { get; set; }
    public string CurrentDecision { get; set; }

    public List<JigsawHouseholdMember> HouseholdComposition { get; set; }
}

public class JigsawHouseholdMember
{
    #nullable enable
    public string? Name { get; set; }
    public string? Gender { get; set; }
    public string? DateOfBirth { get; set; }
    public string? NiNumber { get; set; }
    public string? NhsNumber { get; set; }
    #nullable disable
}


public class AccommodationPlacementInfo
{
    public string PlacementType { get; set; }
    public FullAddressDetails FullAddressDetails { get; set; }
    public string PlacementDuty { get; set; }
    public string PlacementDutyFullName { get; set; }
    public string Usage { get; set; }
    public string DclgClassificationType { get; set; }

}

public class AdditionalInfo
{
    public string Legend { get; set; }
    public List<Information> Info { get; set; }
}

public class Information
{
    public string Question { get; set; }
    public string Answer { get; set; }
}
