using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using SingleViewApi.V1.UseCase;
using Hackney.Core.Testing.Shared;
using Hackney.Shared.ContactDetail.Domain;
using Hackney.Shared.Person;
using Microsoft.OpenApi.Extensions;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetPersonApiByIdUseCaseTests : LogCallAspectFixture
    {
        private Mock<IPersonGateway> _mockPersonGateway;
        private Mock<IContactDetailsGateway> _mockContactDetailsGateway;
        private Mock<IEqualityInformationGateway> _mockEqualityInfoGateway;
        private GetPersonApiByIdUseCase _classUnderTest;
        private Fixture _fixture;
        private Mock<IDataSourceGateway> _mockDataSourceGateway;
        private Mock<ICautionaryAlertsGateway> _mockCautionaryAlertsGateway;

        [SetUp]
        public void SetUp()
        {
            _mockPersonGateway = new Mock<IPersonGateway>();
            _mockContactDetailsGateway = new Mock<IContactDetailsGateway>();
            _mockDataSourceGateway = new Mock<IDataSourceGateway>();
            _mockEqualityInfoGateway = new Mock<IEqualityInformationGateway>();
            _mockCautionaryAlertsGateway = new Mock<ICautionaryAlertsGateway>();

            _classUnderTest = new GetPersonApiByIdUseCase(_mockPersonGateway.Object, _mockContactDetailsGateway.Object, _mockDataSourceGateway.Object, _mockEqualityInfoGateway.Object, _mockCautionaryAlertsGateway.Object);
            _fixture = new Fixture();

        }

        [Test]
        public async Task GetsACustomerFromPersonApi()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();
            var stubbedPerson = _fixture.Create<Person>();
            var stubbedDataSource = _fixture.Create<DataSource>();
            var stubbedContactDetails = new List<ContactDetails>() { _fixture.Create<ContactDetails>() };
            var stubbedEqualityInfo = _fixture.Create<EqualityInformationResponseObject>();
            var stubbedCautionaryAlerts = _fixture.Create<CautionaryAlertResponseObject>();
            _mockPersonGateway.Setup(x => x.GetPersonById(id, userToken)).ReturnsAsync(stubbedPerson);
            _mockContactDetailsGateway.Setup(x => x.GetContactDetailsById(id, userToken)).ReturnsAsync(stubbedContactDetails);
            _mockDataSourceGateway.Setup(x => x.GetEntityById(1)).Returns(stubbedDataSource);
            _mockEqualityInfoGateway.Setup(x => x.GetEqualityInformationById(id, userToken)).ReturnsAsync(stubbedEqualityInfo);
            _mockCautionaryAlertsGateway.Setup(x => x.GetCautionaryAlertsById(id, userToken)).ReturnsAsync(stubbedCautionaryAlerts);

            var result = await _classUnderTest.Execute(id, userToken);

            result.SystemIds[^1].SystemName.Should().BeEquivalentTo(stubbedDataSource.Name);
            result.SystemIds[^1].Id.Should().BeEquivalentTo(id);

            result.Customer.Surname.Should().BeEquivalentTo(stubbedPerson.Surname);
            result.Customer.Surname.Should().BeEquivalentTo(stubbedPerson.Surname);
            result.Customer.Title.Should().BeEquivalentTo(stubbedPerson.Title.GetDisplayName());
            result.Customer.PreferredTitle.Should().BeEquivalentTo(stubbedPerson.PreferredTitle);
            result.Customer.PreferredFirstName.Should().BeEquivalentTo(stubbedPerson.PreferredFirstName);
            result.Customer.PreferredMiddleName.Should().BeEquivalentTo(stubbedPerson.PreferredMiddleName);
            result.Customer.PreferredSurname.Should().BeEquivalentTo(stubbedPerson.PreferredSurname);
            result.Customer.FirstName.Should().BeEquivalentTo(stubbedPerson.FirstName);
            result.Customer.Surname.Should().BeEquivalentTo(stubbedPerson.Surname);
            result.Customer.PlaceOfBirth.Should().BeEquivalentTo(stubbedPerson.PlaceOfBirth);
            result.Customer.DateOfBirth.Should().Be(stubbedPerson.DateOfBirth);
            result.Customer.DateOfDeath.Should().Be(stubbedPerson.DateOfDeath);
            result.Customer.IsAMinor.Should().Be(stubbedPerson.IsAMinor);
            result.Customer.KnownAddresses.Count.Should().Be(stubbedPerson.Tenures.ToList().Count);
            result.Customer.KnownAddresses[0].Id.Should().Be(stubbedPerson.Tenures.ToList()[0].Id);
            result.Customer.KnownAddresses[0].EndDate.Should().Be(stubbedPerson.Tenures.ToList()[0].EndDate);
            result.Customer.KnownAddresses[0].StartDate.Should().Be(stubbedPerson.Tenures.ToList()[0].StartDate);
            result.Customer.KnownAddresses[0].FullAddress.Should().Be(stubbedPerson.Tenures.ToList()[0].AssetFullAddress);
            result.Customer.KnownAddresses[0].CurrentAddress.Should().Be(stubbedPerson.Tenures.ToList()[0].IsActive);
            result.Customer.KnownAddresses[0].DataSourceName.Should().BeEquivalentTo(stubbedDataSource.Name);
            result.Customer.AllContactDetails[0].AddressExtended.Should().BeEquivalentTo(stubbedContactDetails[0].ContactInformation.AddressExtended);
            result.Customer.AllContactDetails[0].ContactType.Should().BeEquivalentTo(stubbedContactDetails[0].ContactInformation.ContactType.ToString());
            result.Customer.AllContactDetails[0].DataSourceName.Should().BeEquivalentTo(stubbedDataSource.Name);
            result.Customer.AllContactDetails[0].Description.Should().BeEquivalentTo(stubbedContactDetails[0].ContactInformation.Description);
            result.Customer.AllContactDetails[0].IsActive.Should().Be(stubbedContactDetails[0].IsActive);
            result.Customer.AllContactDetails[0].SourceServiceArea.Should().BeEquivalentTo(stubbedContactDetails[0].SourceServiceArea.Area);
            result.Customer.AllContactDetails[0].SubType.Should().BeEquivalentTo(stubbedContactDetails[0].ContactInformation.SubType.ToString());
            //result.Customer.EqualityInformation.Should().BeEquivalentTo(stubbedEqualityInfo);
            result.Customer.CautionaryAlerts.Should().BeEquivalentTo(stubbedCautionaryAlerts.Alerts);
        }

        [Test]
        public async Task ReturnsErrorWhenPersonNotfoundInPersonApi()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();
            var stubbedDataSource = _fixture.Create<DataSource>();

            _mockPersonGateway.Setup(x => x.GetPersonById(id, userToken)).ReturnsAsync((Person) null);
            _mockDataSourceGateway.Setup(x => x.GetEntityById(1)).Returns(stubbedDataSource);

            var result = await _classUnderTest.Execute(id, userToken);

            result.SystemIds[^1].SystemName.Should().BeEquivalentTo(stubbedDataSource.Name);
            result.SystemIds[^1].Id.Should().BeEquivalentTo(id);
            result.SystemIds[^1].Error.Should().BeEquivalentTo(SystemId.NotFoundMessage);


        }
    }
}
