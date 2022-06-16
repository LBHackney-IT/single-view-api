using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase;
using Hackney.Core.Testing.Shared;
using Hackney.Shared.ContactDetail.Domain;
using Hackney.Shared.Person;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetCustomerByIdUseCaseTests : LogCallAspectFixture
    {
        private Mock<ICustomerGateway> _mockCustomerGateway;
        private Mock<IGetPersonApiByIdUseCase> _mockGetPersonApiByIdUseCase;
        private Mock<IGetJigsawCustomerByIdUseCase> _mockGetJigsawCustomerByIdUseCase;
        private GetCustomerByIdUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockCustomerGateway = new Mock<ICustomerGateway>();
            _mockGetPersonApiByIdUseCase = new Mock<IGetPersonApiByIdUseCase>();
            _mockGetJigsawCustomerByIdUseCase = new Mock<IGetJigsawCustomerByIdUseCase>();
            _classUnderTest = new GetCustomerByIdUseCase(_mockCustomerGateway.Object, _mockGetPersonApiByIdUseCase.Object, _mockGetJigsawCustomerByIdUseCase.Object);
            _fixture = new Fixture();

        }

        [Test]
        public Task GetsACustomerFromPersonApi()
        {
            var id = new Guid();
            var userToken = _fixture.Create<string>();
            var redisId = _fixture.Create<string>();
            var mockFirstName = "Luna";
            var mockLastName = "Kitty";
            var mockPersonApi = new Guid().ToString();
            var mockJigsawId = "1234";

            _mockCustomerGateway.Setup(x => x.Find(id)).Returns(new SavedCustomer()
            {
                FirstName = mockFirstName, LastName = mockLastName, DataSources = new List<CustomerDataSource>()
                {
                    new CustomerDataSource()
                    {
                        CustomerId = id, DataSourceId = 2, SourceId = mockJigsawId
                    }, new CustomerDataSource()
                    {
                        CustomerId = id, DataSourceId = 1, SourceId = mockPersonApi
                    }
                }
            });
            _mockGetPersonApiByIdUseCase.Setup(x => x.Execute(mockPersonApi, userToken)).ReturnsAsync(new CustomerResponseObject()
            {
                Customer = new Customer()
                {
                    Id = mockPersonApi, Title = Hackney.Shared.Person.Domain.Title.Miss, FirstName = mockFirstName, Surname = mockLastName,
                }
            });
            // _mockDataSourceGateway.Setup(x => x.GetEntityById(1)).Returns(stubbedDataSource);

            var result = _classUnderTest.Execute(id, userToken, redisId);

            result.SystemIds[^1].SystemName.Should().BeEquivalentTo(stubbedDataSource.Name);
            result.SystemIds[^1].Id.Should().BeEquivalentTo(id);

            result.Customer.Surname.Should().BeEquivalentTo(stubbedPerson.Surname);
            result.Customer.Surname.Should().BeEquivalentTo(stubbedPerson.Surname);
            result.Customer.Title.Should().BeEquivalentTo(stubbedPerson.Title);
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
            result.Customer.ContactDetails.Should().BeEquivalentTo(stubbedContactDetails);
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
