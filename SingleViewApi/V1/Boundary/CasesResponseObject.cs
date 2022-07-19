using System;
using System.Collections.Generic;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Boundary;

public class CasesResponseObject
{
#nullable enable
    public List<Case>? Cases { get; set; }
#nullable disable
    public CurrentPlacement CurrentPlacement { get; set; }

}




public class CurrentPlacement
{
    public string PlacementType { get; set; }
    public string Address { get; set; }
    public DateTime StartDate { get; set; }
    public double RentCostCustomer { get; set; }

}
