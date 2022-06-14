using System;
using System.Collections.Generic;
using Hackney.Shared.ContactDetail.Domain;
using Hackney.Shared.Person.Domain;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Boundary.Response
{
    public class CustomerResponseObject
    {
#nullable enable
        public Customer? Customer { get; set; }
#nullable disable
        public List<SystemId> SystemIds { get; set; }

    }

    public class Customer
    {
        public string Id { get; set; }
        public Hackney.Shared.Person.Domain.Title? Title { get; set; }

        public DataSource DataSource { get; set; }

        public Hackney.Shared.Person.Domain.Title? PreferredTitle { get; set; }

        public string PreferredFirstName { get; set; }

        public string PreferredMiddleName { get; set; }

        public string PreferredSurname { get; set; }



        public string FirstName { get; set; }
        public string Surname { get; set; }

        public string PlaceOfBirth { get; set; }

        public List<KnownAddress> KnownAddresses { get; set; }

        public ContactDetails ContactDetails { get; set; }

        public List<PersonType> PersonTypes { get; set; }

#nullable enable
        public DateTime? DateOfBirth { get; set; }

        public string? NiNo { get; set; }

        public string? NhsNumber { get; set; }

        public DateTime? DateOfDeath { get; set; }

        public bool? IsAMinor { get; set; }

#nullable disable


    }
}
