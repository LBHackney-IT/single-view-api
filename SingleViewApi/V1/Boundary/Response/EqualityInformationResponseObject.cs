using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response;

public class CaringResponsibilities
{
#nullable enable
    public string? ProvideUnpaidCare { get; set; }
    public string? HoursSpentProvidingUnpaidCare { get; set; }
#nullable disable
}

public class EconomicSituation
{
#nullable enable
    public string? EconomicSituationValue { get; set; }
    public string? EconomicSituationValueIfOther { get; set; }
#nullable disable
}

public class Ethnicity
{
#nullable enable
    public string? EthnicGroupValue { get; set; }
    public string? EthnicGroupValueIfOther { get; set; }
#nullable disable
}

public class Gender
{
#nullable enable
    public string? GenderValue { get; set; }
    public string? GenderValueIfOther { get; set; }
    public string? GenderDifferentToBirthSex { get; set; }
#nullable disable
}

public class HomeSituation
{
#nullable enable
    public string? HomeSituationValue { get; set; }
    public string? HomeSituationValueIfOther { get; set; }
#nullable disable
}

public class MarriageOrCivilPartnership
{
#nullable enable
    public string? Married { get; set; }
    public string? CivilPartnership { get; set; }
#nullable disable
}

public class PregnancyOrMaternity
{
#nullable enable
    public string? PregnancyDate { get; set; }
    public string? PregnancyValidUntil { get; set; }
#nullable disable
}

public class ReligionOrBelief
{
#nullable enable
    public string? ReligionOrBeliefValue { get; set; }
    public string? ReligionOrBeliefValueIfOther { get; set; }
#nullable disable
}

public class SexualOrientation
{
#nullable enable
    public string? SexualOrientationValue { get; set; }
    public string? SexualOrientationValueIfOther { get; set; }
#nullable disable
}

public class EqualityInformationResponseObject
{
    public string Id { get; set; }
    public string TargetId { get; set; }
#nullable enable
    public string? AgeGroup { get; set; }
    public Gender? Gender { get; set; }
    public string? Nationality { get; set; }
    public Ethnicity? Ethnicity { get; set; }
    public ReligionOrBelief? ReligionOrBelief { get; set; }
    public SexualOrientation? SexualOrientation { get; set; }
    public MarriageOrCivilPartnership? MarriageOrCivilPartnership { get; set; }
    public List<PregnancyOrMaternity>? PregnancyOrMaternity { get; set; }
    public string? NationalInsuranceNumber { get; set; }
    public CaringResponsibilities? CaringResponsibilities { get; set; }
    public string? Disabled { get; set; }
    public List<string>? CommunicationRequirements { get; set; }
    public EconomicSituation? EconomicSituation { get; set; }
    public HomeSituation? HomeSituation { get; set; }
    public string? ArmedForces { get; set; }
#nullable disable
}
