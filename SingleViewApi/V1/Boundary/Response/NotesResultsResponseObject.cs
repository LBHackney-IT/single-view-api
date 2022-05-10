using System.Collections.Generic;
using Hackney.Core.DynamoDb;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Boundary.Response
{
    public class NotesResultsResponseObject
    {
        public List<NoteResponseObject> Results { get; set; }

        public PaginationDetails PaginationDetails { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
