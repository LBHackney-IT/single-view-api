using System;
using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response;

public class JigsawCustomerSearchApiResponseObject
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
#nullable enable
    public string? Address { get; set; }
    public string? MobilePhone { get; set; }
    public string? HomePhone { get; set; }
    public string? NickName { get; set; }
    public string? Email { get; set; }
    public string? NhsNumber { get; set; }
    public string? NiNumber { get; set; }
#nullable disable
    public int? EntityTypeId { get; set; }
    public DateTime DoB { get; set; }
    public string DisplayName { get; set; }
    public string HouseholdMembers { get; set; }
    public string FormattedHouseholdMembers { get; set; }
    public List<string> HouseholdMembersList { get; set; }

}
