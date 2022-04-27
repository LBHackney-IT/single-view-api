using System.Collections.Generic;
using Hackney.Shared.Person;

namespace SingleViewApi.V1.Boundary.Response
{
    public class Results
    {
        public List<Person> Persons { get; set; }
    }

    public class HousingSearchApiResponse
    {
        public Results Results { get; set; }
        public int Total { get; set; }
    }


}
