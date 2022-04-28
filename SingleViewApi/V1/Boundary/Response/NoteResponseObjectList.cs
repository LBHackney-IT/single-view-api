using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response
{
    public class NoteResponseObjectList
    {
        public List<NoteResponseObject> NoteResponseObjects { get; set; }

        public void SortByCreatedAtDescending()
        {
            NoteResponseObjects.Sort((x, y) =>
                y.CreatedAt.CompareTo(x.CreatedAt));
        }
    }
}
