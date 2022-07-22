using System;
using System.Collections.Generic;

namespace SingleViewApi.V1.Domain;

public class CautionaryAlert
{
    public string DateModified { get; set; }
    public string ModifiedBy { get; set; }
    public string StartDate { get; set; }
    #nullable enable
    public string? EndDate { get; set; }
    #nullable disable
    public string AlertCode { get; set; }
    public string Description { get; set; }
    public string Reason { get; set; }
}
public class CautionaryAlertResponseObject
{
    public string PersonId { get; set; }
    public List<CautionaryAlert> Alerts { get; set; }
}

