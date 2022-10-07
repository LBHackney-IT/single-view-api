
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Boundary
{
    public class SystemIdList
    {
        public List<SystemId> SystemIds { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(SystemIds);
        }
    }
}
