using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Boundary
{
    public class SystemId
    {
        // TODO: Rename to DataSource
        public DataSource SystemName { get; set; }
        public string Id { get; set; }
#nullable enable
        public string? Error { get; set; }
#nullable disable
    }
}
