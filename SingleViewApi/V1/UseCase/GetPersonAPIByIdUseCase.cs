using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Hackney.Shared.ContactDetail.Domain;
using Hackney.Shared.Person;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.UseCase
{
    public class GetPersonApiByIdUseCase : IGetPersonApiByIdUseCase
    {
        private readonly IPersonGateway _personGateway;
        private readonly IContactDetailsGateway _contactDetailsGateway;
        private readonly IDataSourceGateway _dataSourceGateway;
        private readonly IEqualityInformationGateway _equalityInformationGateway;

        public GetPersonApiByIdUseCase(IPersonGateway personGateway, IContactDetailsGateway contactDetailsGateway,
            IDataSourceGateway dataSourceGateway, IEqualityInformationGateway equalityInformationGateway)
        {
            _personGateway = personGateway;
            _contactDetailsGateway = contactDetailsGateway;
            _dataSourceGateway = dataSourceGateway;
            _equalityInformationGateway = equalityInformationGateway;
        }
        [LogCall]
        public async Task<CustomerResponseObject> Execute(string personId, string userToken)
        {
            Console.WriteLine("---- DEBUG - Entered Use Case - querying for person {0}", personId);

            var person = new Person();
            var contactDetails = new ContactDetails();
            var dataSource = new DataSource();

            try
            {
                person = await _personGateway.GetPersonById(personId, userToken);
                Console.WriteLine("---- DEBUG - Got Person with ID {0}", personId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in person gateway: {0}", e.ToString());
            }

            try
            {
                contactDetails = await _contactDetailsGateway.GetContactDetailsById(personId, userToken);
                Console.WriteLine("---- DEBUG - Got Contact Details");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in contact details gateway: {0}", e.ToString());
            }

            try
            {
                dataSource = _dataSourceGateway.GetEntityById(1);
                Console.WriteLine("---- DEBUG - Got Data Source");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in dataSOurce gateway: {0}", e.ToString());
            }




            Console.WriteLine("---- DEBUG - GETTING EQUALITY INFORMATION");
            var equalityInformation =
            await _equalityInformationGateway.GetEqualityInformationById(personId, userToken);
            Console.WriteLine("----- DEBUG - Equality Information is {0}", equalityInformation.ToString());


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
