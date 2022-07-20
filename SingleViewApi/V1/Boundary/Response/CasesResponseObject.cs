using System;
using System.Collections.Generic;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Boundary;

public class CasesResponseObject
{
#nullable enable
    public Case? CurrentCase { get; set; }
#nullable disable
    public List<AccommodationPlacementInfo> PlacementInformation { get; set; }

    public List<CaseOverview> CaseOverviews { get; set; }

}




public class CaseOverview
{
    public string Id { get; set; }
    public string CurrentFlowchartPosition { get; set; }
    public string CurrentDecision { get; set; }
    public string HouseHoldComposition { get; set; }
}


public class AccommodationPlacementInfo
{
    public string PlacementType { get; set; }
    public FullAddressDetails FullAddressDetails { get; set; }
    public string PlacementDuty { get; set; }
    public string PLacementDutyFullName { get; set; }
    public string Usage { get; set; }
    public string DclgClassificationType { get; set; }

}

