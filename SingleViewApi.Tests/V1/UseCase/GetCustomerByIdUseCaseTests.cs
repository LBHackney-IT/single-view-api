using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase;
using Hackney.Core.Testing.Shared;
using Hackney.Shared.Person;
using Moq;
using NUnit.Framework;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetPersonByIdUseCaseTests : LogCallAspectFixture
    {
        private Mock<IPersonGateway> _mockPersonGateway;
        private GetCustomerByIdUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockPersonGateway = new Mock<IPersonGateway>();
            _classUnderTest = new GetCustomerByIdUseCase(_mockPersonGateway.Object);
            _fixture = new Fixture();

        }

        [Test]
        public async Task GetsACustomerFromPersonApi()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();
            var stubbedEntity = _fixture.Create<Person>();
            _mockPersonGateway.Setup(x => x.GetPersonById(id, userToken)).ReturnsAsync(stubbedEntity);

            var result = await _classUnderTest.Execute(id, userToken);

            result.SystemIds[^1].SystemName.Should().BeEquivalentTo("PersonApi");
            result.SystemIds[^1].Id.Should().BeEquivalentTo(id);

            result.Customer.Surname.Should().BeEquivalentTo(stubbedEntity.Surname);
            result.Customer.Surname.Should().BeEquivalentTo(stubbedEntity.Surname);
            result.Customer.Title.Should().BeEquivalentTo(stubbedEntity.Title);
            result.Customer.PreferredTitle.Should().BeEquivalentTo(stubbedEntity.PreferredTitle);
            result.Customer.PreferredFirstName.Should().BeEquivalentTo(stubbedEntity.PreferredFirstName);
            result.Customer.PreferredMiddleName.Should().BeEquivalentTo(stubbedEntity.PreferredMiddleName);
            result.Customer.PreferredSurname.Should().BeEquivalentTo(stubbedEntity.PreferredSurname);
            result.Customer.FirstName.Should().BeEquivalentTo(stubbedEntity.FirstName);
            result.Customer.MiddleName.Should().BeEquivalentTo(stubbedEntity.MiddleName);
            result.Customer.Surname.Should().BeEquivalentTo(stubbedEntity.Surname);
            result.Customer.PlaceOfBirth.Should().BeEquivalentTo(stubbedEntity.PlaceOfBirth);
            result.Customer.DateOfBirth.Should().Be(stubbedEntity.DateOfBirth);
            result.Customer.DateOfDeath.Should().Be(stubbedEntity.DateOfDeath);
            result.Customer.IsAMinor.Should().Be(stubbedEntity.IsAMinor);
            result.Customer.KnownAddresses.Count.Should().Be(stubbedEntity.Tenures.ToList().Count);
            result.Customer.KnownAddresses[0].Id.Should().Be(stubbedEntity.Tenures.ToList()[0].Id);
            result.Customer.KnownAddresses[0].EndDate.Should().Be(stubbedEntity.Tenures.ToList()[0].EndDate);
            result.Customer.KnownAddresses[0].StartDate.Should().Be(stubbedEntity.Tenures.ToList()[0].StartDate);
            result.Customer.KnownAddresses[0].FullAddress.Should().Be(stubbedEntity.Tenures.ToList()[0].AssetFullAddress);
            result.Customer.KnownAddresses[0].CurrentAddress.Should().Be(stubbedEntity.Tenures.ToList()[0].IsActive);
        }
        [Test]
        public async Task ReturnsErrorWhenPersonNotfoundInPersonApi()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();
            _mockPersonGateway.Setup(x => x.GetPersonById(id, userToken)).ReturnsAsync((Person) null);

            var result = await _classUnderTest.Execute(id, userToken);

            result.SystemIds[^1].SystemName.Should().BeEquivalentTo("PersonApi");
            result.SystemIds[^1].Id.Should().BeEquivalentTo(id);
            result.SystemIds[^1].Error.Should().BeEquivalentTo("Not found");


        }
    }
}
