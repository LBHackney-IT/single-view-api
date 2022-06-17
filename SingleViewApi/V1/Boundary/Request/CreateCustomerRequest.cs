using System;
using System.Collections.Generic;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Boundary.Request
{
    public class CreateCustomerRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string NiNumber { get; set; }

        public List<DataSourceRequest> DataSources { get; set; }
    }

    public class DataSourceRequest
    {
        public string DataSource { get; set; }
        public string SourceId { get; set; }

    }
}
