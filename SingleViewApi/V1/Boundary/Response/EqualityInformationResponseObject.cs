using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SingleViewApi.V1.Boundary.Response;

public class CaringResponsibilities
{
    public string ProvideUnpaidCare { get; set; }
    public string HoursSpentProvidingUnpaidCare { get; set; }
}

public class EconomicSituation
{
    public string EconomicSituationValue { get; set; }
    public string EconomicSituationValueIfOther { get; set; }
}

public class Ethnicity
{
    public string EthnicGroupValue { get; set; }
    public string EthnicGroupValueIfOther { get; set; }
}

public class Gender
{
    public string GenderValue { get; set; }
    public string GenderValueIfOther { get; set; }
    public bool GenderDifferentToBirthSex { get; set; }
}

public class HomeSituation
{
    public string HomeSituationValue { get; set; }
    public string HomeSituationValueIfOther { get; set; }
}

public class Language
{
    [JsonPropertyName("Language")]
    public string LanguageName { get; set; }
    public bool IsPrimary { get; set; }
}

public class MarriageOrCivilPartnership
{
    public string Married { get; set; }
    public string CivilPartnership { get; set; }
}

public class PregnancyOrMaternity
{
    public string PregnancyDate { get; set; }
    public string PregnancyValidUntil { get; set; }
}

public class ReligionOrBelief
{
    public string ReligionOrBeliefValue { get; set; }
    public string ReligionOrBeliefValueIfOther { get; set; }
}

public class SexualOrientation
{
    public string SexualOrientationValue { get; set; }
    public string SexualOrientationValueIfOther { get; set; }
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
    public List<Language>? Languages { get; set; }
    public CaringResponsibilities? CaringResponsibilities { get; set; }
    public bool? Disabled { get; set; }
    public List<string>? CommunicationRequirements { get; set; }
    public EconomicSituation? EconomicSituation { get; set; }
    public HomeSituation? HomeSituation { get; set; }
    public string? ArmedForces { get; set; }
#nullable disable
}




