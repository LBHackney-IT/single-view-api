using System;
using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response;

public class JigsawCasesResponseObject
{
    public List<Case> Cases { get; set; }
}

public class Case
{
    public int Id { get; set; }
    public string StatusName { get; set; }
    public DateTime DateOfApproach { get; set; }
    public bool IsCurrent { get; set; }
    public string AssignedTo { get; set; }
    public bool IsV2LegacyCase { get; set; }
}
