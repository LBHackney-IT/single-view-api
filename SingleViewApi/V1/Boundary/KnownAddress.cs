using System;

namespace SingleViewApi.V1.Boundary
{
    public class KnownAddress
    {
        public string FullAddress { get; set; }
        public Guid Id { get; set; }
        public bool CurrentAddress { get; set; }
#nullable enable
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
#nullable disable
        public string DataSourceName { get; set; }
    }
}
