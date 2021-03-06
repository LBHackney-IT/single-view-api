using System.Collections.Generic;
using System.Linq;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Factories
{
    public static class ResponseFactory
    {
        //TODO: Map the fields in the domain object(s) to fields in the response object(s).
        // More information on this can be found here https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Factory-object-mappings
        public static ResponseObject ToResponse(this DataSource domain)
        {
            return new ResponseObject();
        }

        public static List<ResponseObject> ToResponse(this IEnumerable<DataSource> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
