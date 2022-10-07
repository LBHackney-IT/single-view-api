using System.Collections.Generic;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Boundary
{
    public class NotesResponse
    {
        public List<NoteResponseObject> Notes { get; set; }

        public List<SystemId> SystemIds { get; set; }

        public void Sort()
        {
            SortByCreatedAtDescending();
        }

        public void SortByCreatedAtDescending()
        {
            Notes.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));
        }
    }
}
