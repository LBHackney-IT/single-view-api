using System;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Boundary.Response
{
    public class NotesApiResponseObject
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

        public static NotesApiResponseObject Create(Note note)
        {
            if (note is null) throw new ArgumentNullException(nameof(note));

            return new NotesApiResponseObject
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
    }
}
