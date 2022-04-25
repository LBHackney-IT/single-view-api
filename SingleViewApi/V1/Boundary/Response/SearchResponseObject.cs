using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response
{
    public class SearchResponseObject
    {

       public SearchResponse SearchResponse { get; set; }

       public List<SystemId> SystemIds { get; set; } }

    public class SearchResponse

    {
        public HousingSearchApiResponse Results { get; set; }
    }


}
