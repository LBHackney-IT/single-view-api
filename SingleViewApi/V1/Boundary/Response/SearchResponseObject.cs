using System;
using System.Collections.Generic;
using Hackney.Shared.Person;

namespace SingleViewApi.V1.Boundary.Response
{
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
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string SurName { get; set; }
#nullable enable
        public Hackney.Shared.Person.Domain.Title? Title { get; set; }
        public string? MiddleName { get; set; }
        public string? PreferredFirstName { get; set; }
        public string? PreferredSurname { get; set; }
        public DateTime? DateOfBirth { get; set; }
#nullable disable
        public List<Hackney.Shared.Person.Domain.PersonType> PersonTypes { get; set; }
        public bool IsPersonCautionaryAlert { get; set; }
        public bool IsTenureCautionaryAlert { get; set; }
        public List<KnownAddress> KnownAddresses { get; set; }

    }


}
