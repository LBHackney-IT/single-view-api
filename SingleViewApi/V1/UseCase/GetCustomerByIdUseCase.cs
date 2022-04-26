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
    public class GetCustomerByIdUseCase : IGetCustomerByIdUseCase
    {
        private IPersonGateway _personGateway;
        public GetCustomerByIdUseCase(IPersonGateway personGateway)
        {
            _personGateway = personGateway;
        }
        [LogCall]
        public async Task<CustomerResponseObject> Execute(string personId, string userToken)
        {
            var person = await _personGateway.GetPersonById(personId, userToken);
            var personApiId = new SystemId() { SystemName = "PersonApi", Id = personId };

            var response = new CustomerResponseObject()
            {
                SystemIds = new List<SystemId>() { personApiId }
            };

            if (person == null)
            {
                personApiId.Error = "Not found";
            }
            else
            {
                response.Customer = new Customer()
                {
                    Title = person.Title,
                    PreferredTitle = person.PreferredTitle,
                    PreferredFirstName = person.PreferredFirstName,
                    PreferredMiddleName = person.PreferredMiddleName,
                    PreferredSurname = person.PreferredSurname,
                    FirstName = person.FirstName,
                    MiddleName = person.MiddleName,
                    Surname = person.Surname,
                    PlaceOfBirth = person.PlaceOfBirth,
                    DateOfBirth = person.DateOfBirth,
                    DateOfDeath = person.DateOfDeath,
                    IsAMinor = person.IsAMinor,
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
