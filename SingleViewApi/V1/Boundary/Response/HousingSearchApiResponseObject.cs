using System;
using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response
{

    public class HousingSearchApiResponseObject
    {
#nullable enable

        public HousingSearchApiResponse? HousingSearchResponse { get; set; }

#nullable disable
        public List<SystemId> SystemIds { get; set; }
    }
    public class Tenure
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public double TotalBalance { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AssetFullAddress { get; set; }
        public string PostCode { get; set; }
        public string PaymentReference { get; set; }
        public bool IsActive { get; set; }
    }

    public class HousingSearchPerson
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Firstname { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string PreferredFirstname { get; set; }
        public string PreferredSurname { get; set; }
        public double TotalBalance { get; set; }
        public string DateOfBirth { get; set; }
        public List<string> PersonTypes { get; set; }
        public bool IsPersonCautionaryAlert { get; set; }
        public bool IsTenureCautionaryAlert { get; set; }
        public List<Tenure> Tenures { get; set; }
    }

    public class HousingSearchApiResponse
    {
        public List<HousingSearchPerson> HousingSearchPersons { get; set; }
        public int Total { get; set; }
    }
}
