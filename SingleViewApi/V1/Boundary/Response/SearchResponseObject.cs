using System;
using System.Collections.Generic;
using Hackney.Shared.Person;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Boundary.Response
{
    public class GetPersonsResponse
    {
        public List<Person> Persons { get; set; }
        public DateTime LastUpdated { get; set; }
    }
    public class SearchResponseObject
    {

        public SearchResponse SearchResponse { get; set; }

        public List<SystemId> SystemIds { get; set; }
    }

    public class SearchResponse

    {
        public List<SearchResult> SearchResults { get; set; }
        public int Total { get; set; }
    }

    public class SearchResult
    {
        public string Id { get; set; }
        public string DataSource { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
#nullable enable
        public Hackney.Shared.Person.Domain.Title? Title { get; set; }
        public string? MiddleName { get; set; }
        public string? PreferredFirstName { get; set; }
        public string? PreferredSurname { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? NiNumber { get; set; }
        public List<Hackney.Shared.Person.Domain.PersonType>? PersonTypes { get; set; }
        public bool IsPersonCautionaryAlert { get; set; }
        public bool IsTenureCautionaryAlert { get; set; }
        public List<KnownAddress>? KnownAddresses { get; set; }
#nullable disable
    }
}
