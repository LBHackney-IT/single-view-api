using System;

namespace SingleViewApi.V1.Domain
{
    public class CustomerDataSource
    {
        public int Id { get; set; }

        public Guid CustomerId { get; set; }

        public int DataSourceId { get; set; }

        public string SourceId { get; set; }
    }
}
