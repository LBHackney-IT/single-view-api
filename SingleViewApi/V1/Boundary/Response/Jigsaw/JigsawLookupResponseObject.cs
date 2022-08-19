// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

using System.Collections.Generic;
using Newtonsoft.Json;

public class AccommodationType
{
    [JsonProperty("sortOrder")]
    public int SortOrder { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("countryId")]
    public int CountryId { get; set; }
}


public class GenderLookup
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("countryId")]
    public int CountryId { get; set; }
}

public class HousingCircumstance
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("countryId")]
    public int CountryId { get; set; }
}

public class PersonTitle
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("countryId")]
    public int CountryId { get; set; }
}

public class PublicBodiesAlert
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("countryId")]
    public int CountryId { get; set; }
}

public class Relationship
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("countryId")]
    public int CountryId { get; set; }
}

public class JigsawLookupResponseObject
{
    [JsonProperty("personTitles")]
    public List<PersonTitle> PersonTitles { get; set; }

    [JsonProperty("genders")]
    public List<GenderLookup> GenderLookup { get; set; }

    [JsonProperty("publicBodiesALERT")]
    public List<PublicBodiesAlert> PublicBodiesAlert { get; set; }

    [JsonProperty("relationships")]
    public List<Relationship> Relationships { get; set; }

    [JsonProperty("accommodationTypes")]
    public List<AccommodationType> AccommodationTypes { get; set; }

    [JsonProperty("housingCircumstances")]
    public List<HousingCircumstance> HousingCircumstances { get; set; }
}

