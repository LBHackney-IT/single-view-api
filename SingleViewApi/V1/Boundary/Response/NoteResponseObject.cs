using System;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Boundary.Response
{
    public class NoteResponseObject : IComparable<NoteResponseObject>
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public TargetType TargetType { get; set; }

        public Guid TargetId { get; set; }

        public DateTime CreatedAt { get; set; }

        public Categorisation Categorisation { get; set; }

        public AuthorDetails Author { get; set; }

        public bool Highlight { get; set; }

        public static NoteResponseObject Create(Note note)
        {
            if (note is null) throw new ArgumentNullException(nameof(note));

            return new NoteResponseObject
            {
                Id = note.Id,
                Title = note.Title,
                Description = note.Description,
                TargetType = note.TargetType,
                TargetId = note.TargetId,
                CreatedAt = note.CreatedAt,
                Categorisation = new Categorisation
                {
                    Description = note.Categorisation?.Description,
                    Category = note.Categorisation?.Category,
                    SubCategory = note.Categorisation?.SubCategory
                },
                Author = new AuthorDetails
                {
                    Email = note.Author?.Email,
                    FullName = note.Author?.FullName
                },
                Highlight = note.Highlight
            };
        }

        public int CompareTo(NoteResponseObject other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return CreatedAt.CompareTo(other.CreatedAt);
        }
    }
}
