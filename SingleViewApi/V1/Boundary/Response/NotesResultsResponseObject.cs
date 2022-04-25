using System.Collections.Generic;
using Hackney.Core.DynamoDb;

namespace SingleViewApi.V1.Boundary.Response
{
    public class NotesResultsResponseObject
    {
        public List<NoteResponseObject> Results { get; set; }

        public PaginationDetails PaginationDetails { get; set; }
    }
}
