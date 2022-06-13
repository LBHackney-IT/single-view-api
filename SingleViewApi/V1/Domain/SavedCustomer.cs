using System;
using System.Collections.Generic;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.V1.Domain
{
    public class SavedCustomer
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string NiNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<CustomerDataSource> DataSources { get; set; }

    }
}
