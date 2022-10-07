using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.OpenApi.Extensions;
using ServiceStack;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetPersonApiByIdUseCase : IGetPersonApiByIdUseCase
    {
        private readonly IPersonGateway _personGateway;
        private readonly IContactDetailsGateway _contactDetailsGateway;
        private readonly IDataSourceGateway _dataSourceGateway;
        private readonly IEqualityInformationGateway _equalityInformationGateway;
        private readonly ICautionaryAlertsGateway _cautionaryAlertsGateway;
        private readonly ITenureGateway _tenureGateway;

        public GetPersonApiByIdUseCase(
            IPersonGateway personGateway,
            IContactDetailsGateway contactDetailsGateway,
            IDataSourceGateway dataSourceGateway,
            IEqualityInformationGateway equalityInformationGateway,
            ICautionaryAlertsGateway cautionaryAlertsGateway,
            ITenureGateway tenureGateway)
        {
            _personGateway = personGateway;
            _contactDetailsGateway = contactDetailsGateway;
            _dataSourceGateway = dataSourceGateway;
            _equalityInformationGateway = equalityInformationGateway;
            _cautionaryAlertsGateway = cautionaryAlertsGateway;
            _tenureGateway = tenureGateway;
        }
        [LogCall]
        public async Task<CustomerResponseObject> Execute(string personId, string userToken)
        {

            var person = await _personGateway.GetPersonById(personId, userToken);
            var contactDetails = await _contactDetailsGateway.GetContactDetailsById(personId, userToken);
            var dataSource = _dataSourceGateway.GetEntityById(1);
            var equalityInformation = await _equalityInformationGateway.GetEqualityInformationById(personId, userToken);
            var cautionaryAlerts = await _cautionaryAlertsGateway.GetCautionaryAlertsById(personId, userToken);

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

                var personTypes = new List<string>();

                foreach (var personType in person.PersonTypes)
                {
                    personTypes.Add(personType.ToDescription());
                }

                List<KnownAddress> knownAddresses = person.Tenures.Map(t =>
                {
                    var tenureInfo = _tenureGateway.GetTenureInformation(t.Id, userToken).Result;
                    return new KnownAddress()
                    {
                        Id = t.Id,
                        CurrentAddress = t.IsActive,
                        StartDate = t.StartDate,
                        EndDate = t.EndDate,
                        FullAddress = t.AssetFullAddress,
                        DataSourceName = dataSource.Name,
                        HouseholdMembers = tenureInfo?.HouseholdMembers,
                        LegacyReferences = tenureInfo?.LegacyReferences
                    };
                });

                response.Customer = new Customer()
                {
                    Id = person.Id.ToString(),
                    Title = person.Title?.GetDisplayName(),
                    DataSource = dataSource,
                    PreferredTitle = person.PreferredTitle?.GetDisplayName(),
                    PreferredFirstName = person.PreferredFirstName,
                    PreferredMiddleName = person.PreferredMiddleName,
                    PreferredSurname = person.PreferredSurname,
                    FirstName = person.FirstName,
                    Surname = person.Surname,
                    PlaceOfBirth = person.PlaceOfBirth,
                    DateOfBirth = person.DateOfBirth,
                    DateOfDeath = person.DateOfDeath,
                    IsAMinor = person.IsAMinor,
                    PersonTypes = personTypes,
                    AllContactDetails = contactDetails.Map(c => new CustomerContactDetails()
                    {
                        AddressExtended = c.ContactInformation.AddressExtended,
                        ContactType = c.ContactInformation.ContactType.ToString(),
                        DataSourceName = dataSource.Name,
                        Description = c.ContactInformation.Description,
                        IsActive = c.IsActive,
                        SourceServiceArea = c.SourceServiceArea.Area,
                        SubType = c.ContactInformation.SubType.ToString(),
                        Value = c.ContactInformation.Value,
                    }),
                    EqualityInformation = equalityInformation,
                    CautionaryAlerts = cautionaryAlerts.Alerts,
                    KnownAddresses = knownAddresses
                };
            }

            return response;
        }
    }
}
