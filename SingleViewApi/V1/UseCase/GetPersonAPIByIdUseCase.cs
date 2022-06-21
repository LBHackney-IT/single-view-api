using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.V1.UseCase
{
    public class GetPersonApiByIdUseCase : IGetPersonApiByIdUseCase
    {
        private IPersonGateway _personGateway;
        private IContactDetailsGateway _contactDetailsGateway;
        private readonly IDataSourceGateway _dataSourceGateway;
        //private readonly IEqualityInformationGateway _equalityInformationGateway;

        public GetPersonApiByIdUseCase(IPersonGateway personGateway, IContactDetailsGateway contactDetailsGateway,
            IDataSourceGateway dataSourceGateway)
        {
            _personGateway = personGateway;
            _contactDetailsGateway = contactDetailsGateway;
            _dataSourceGateway = dataSourceGateway;
            //_equalityInformationGateway = equalityInformationGateway;
        }
        [LogCall]
        public async Task<CustomerResponseObject> Execute(string personId, string userToken)
        {
            Console.WriteLine("---- DEBUG - Entered Use Case - querying for person {0}", personId);
            var person = await _personGateway.GetPersonById(personId, userToken);
            Console.WriteLine("---- DEBUG - Got Person with ID {0}", personId);
            var contactDetails = await _contactDetailsGateway.GetContactDetailsById(personId, userToken);
            Console.WriteLine("---- DEBUG - Got Contact Details");
            var dataSource = _dataSourceGateway.GetEntityById(1);
            Console.WriteLine("---- DEBUG - Got Data Source");
            Console.WriteLine("---- DEBUG - GETTING EQUALITY INFORMATION");
            //var equalityInformation =
            // await _equalityInformationGateway.GetEqualityInformationById(personId, userToken);
            //Console.WriteLine("----- DEBUG - Equality Information is {0}", equalityInformation.ToString());


            var personApiId = new SystemId() { SystemName = dataSource.Name, Id = personId };

            var response = new CustomerResponseObject()
            {
                SystemIds = new List<SystemId>() { personApiId }
            };

            if (person == null)
            {
                personApiId.Error = SystemId.NotFoundMessage;
            }
            else
            {
                response.Customer = new Customer()
                {
                    Id = person.Id.ToString(),
                    Title = person.Title,
                    DataSource = dataSource,
                    PreferredTitle = person.PreferredTitle,
                    PreferredFirstName = person.PreferredFirstName,
                    PreferredMiddleName = person.PreferredMiddleName,
                    PreferredSurname = person.PreferredSurname,
                    FirstName = person.FirstName,
                    Surname = person.Surname,
                    PlaceOfBirth = person.PlaceOfBirth,
                    DateOfBirth = person.DateOfBirth,
                    DateOfDeath = person.DateOfDeath,
                    IsAMinor = person.IsAMinor,
                    PersonTypes = person.PersonTypes?.ToList(),
                    ContactDetails = contactDetails,

                    KnownAddresses = new List<KnownAddress>(person.Tenures.Select(t => new KnownAddress()
                    {

                        Id = t.Id,
                        CurrentAddress = t.IsActive,
                        StartDate = t.StartDate,
                        EndDate = t.EndDate,
                        FullAddress = t.AssetFullAddress
                    }))
                };
            }

            return response;
        }
    }
}
