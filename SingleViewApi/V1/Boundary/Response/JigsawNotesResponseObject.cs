using System;

namespace SingleViewApi.V1.Boundary.Response;

public class JigsawNotesResponseObject
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string Content { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime InterviewDate { get; set; }

    public bool IsSensitive { get; set; }

    public bool IsPinned { get; set; }

    public int NoteTypeId { get; set; }

    public int OfficerId { get; set; }

    public string OfficerName { get; set; }

    public string OfficerInitials { get; set; }

#nullable enable
    public string? CustomerName { get; set; }
#nullable disable
}
