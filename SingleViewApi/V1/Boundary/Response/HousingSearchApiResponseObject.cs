using System;
using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary.Response
{

    public class Tenure
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public double TotalBalance { get; set; }
        public string StartDate { get; set; }
#nullable enable
        public string? EndDate { get; set; }
#nullable disable
        public string AssetFullAddress { get; set; }
        public object PostCode { get; set; }
        public string PaymentReference { get; set; }
        public bool IsActive { get; set; }
    }

    public class Person
    {
        public Guid Id { get; set; }
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

    public class Results
    {
        public List<Person> Persons { get; set; }
    }

    public class HousingSearchApiResponse
    {
        public Results Results { get; set; }
        public int Total { get; set; }
    }


}
