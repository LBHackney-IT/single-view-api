using System;
using Newtonsoft.Json;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Boundary.Response;

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

    public bool IsSensitive { get; set; }

    public bool IsPinned { get; set; }

    public string DataSourceId { get; set; }

    public string DataSource { get; set; }

#nullable enable
    public string? JigsawCaseReferenceId { get; set; }
#nullable disable

    public int CompareTo(NoteResponseObject other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return CreatedAt.CompareTo(other.CreatedAt);
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
