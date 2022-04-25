using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response
{
    public class SearchResponseObject
    {
        #nullable enable

        public SearchResponse? SearchResponse { get; set; }

        #nullable disable
        public List<SystemId> SystemIds { get; set; }
    }

    public class SearchResponse

    {
        public HousingSearchApiResponse Results { get; set; }
    }

    //TODO: What fields do we need?
}
