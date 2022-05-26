using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Boundary
{
    public class SystemId
    {
        public static readonly string NotFoundMessage = "Not found";
        public string SystemName { get; set; }
        public string Id { get; set; }
#nullable enable
        public string? Error { get; set; }
#nullable disable
    }
}
