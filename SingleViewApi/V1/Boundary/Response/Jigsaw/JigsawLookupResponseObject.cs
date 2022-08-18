using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response;

public class JigsawLookupResponseObject
{
    public HousingCircumstances HousingCircumstances { get; set; }
    public AccommodationTypes AccommodationTypes { get; set; }
}



public class HousingCircumstances
{
    public List<Lookup> Circumstances { get; set; }
}

public class AccommodationTypes
{
    public List<Lookup> Types { get; set; }
}

public class Lookup
{
    public string Id { get; set; }
    public string Name { get; set; }
}


