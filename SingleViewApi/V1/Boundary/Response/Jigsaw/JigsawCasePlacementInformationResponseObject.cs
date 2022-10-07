using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Boundary.Response;

public class FullAddressDetails
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("houseName")] public string HouseName { get; set; }

    [JsonProperty("houseNumber")] public string HouseNumber { get; set; }

    [JsonProperty("street")] public string Street { get; set; }

    [JsonProperty("town")] public string Town { get; set; }

    [JsonProperty("county")] public string County { get; set; }

    [JsonProperty("postcode")] public string Postcode { get; set; }

#nullable enable
    [JsonProperty("apartment")] public string? Apartment { get; set; }

    [JsonProperty("roomNumber")] public string? RoomNumber { get; set; }

    [JsonProperty("locality")] public string? Locality { get; set; }

    [JsonProperty("latitude")] public string? Latitude { get; set; }

    [JsonProperty("longitude")] public string? Longitude { get; set; }

    [JsonProperty("freetext")] public string? Freetext { get; set; }

#nullable disable
}

public class Placement
{
    [JsonProperty("accommodationId")] public int AccommodationId { get; set; }

    [JsonProperty("tenancyId")] public int TenancyId { get; set; }

    [JsonProperty("placementType")] public string PlacementType { get; set; }

    [JsonProperty("address")] public string Address { get; set; }

    [JsonProperty("fullAddressDetails")] public FullAddressDetails FullAddressDetails { get; set; }

    [JsonProperty("placementDuty")] public string PlacementDuty { get; set; }

    [JsonProperty("placementDutyFullName")]
    public string PlacementDutyFullName { get; set; }

    [JsonProperty("startDate")] public DateTime StartDate { get; set; }

    [JsonProperty("endDate")] public DateTime? EndDate { get; set; }

    [JsonProperty("canEdit")] public bool CanEdit { get; set; }

    [JsonProperty("rentCostCustomer")] public double RentCostCustomer { get; set; }

    [JsonProperty("usageTypeId")] public int UsageTypeId { get; set; }

    [JsonProperty("usage")] public string Usage { get; set; }

    [JsonProperty("rentReference")] public string RentReference { get; set; }

    [JsonProperty("bedrooms")] public int Bedrooms { get; set; }

    [JsonProperty("localAuthority")] public string LocalAuthority { get; set; }

    [JsonProperty("dclgClassificationType")]
    public string DclgClassificationType { get; set; }

    [JsonProperty("depositInScheme")] public bool DepositInScheme { get; set; }

#nullable enable

    [JsonProperty("propertyType")] public string? PropertyType { get; set; }

    [JsonProperty("bondAmount")] public int? BondAmount { get; set; }

    [JsonProperty("bondEndDate")] public string? BondEndDate { get; set; }

    [JsonProperty("deposit")] public int? Deposit { get; set; }

    [JsonProperty("placementTypeDetails")] public string? PlacementTypeDetails { get; set; }

    [JsonProperty("placementDutyDetails")] public string? PlacementDutyDetails { get; set; }

#nullable disable
}

public class JigsawCasePlacementInformationResponseObject
{
    [JsonProperty("placements")] public List<Placement> Placements { get; set; }

    [JsonProperty("isCurrentlyInPlacement")]
    public bool IsCurrentlyInPlacement { get; set; }
}
