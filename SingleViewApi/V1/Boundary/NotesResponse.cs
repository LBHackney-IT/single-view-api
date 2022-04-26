using System.Collections.Generic;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Boundary
{
    public class NotesResponse
    {
        public NoteResponseObjectList Notes { get; set; }

        public List<SystemId> SystemIds { get; set; }
    }
}
