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
using Hackney.Shared.Person.Domain;
using Moq;
using NUnit.Framework;
using ServiceStack;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetCustomerByIdUseCaseTests : LogCallAspectFixture
    {
        private Mock<ICustomerGateway> _mockCustomerGateway;
        private Mock<IGetPersonApiByIdUseCase> _mockGetPersonApiByIdUseCase;
        private Mock<IGetJigsawCustomerByIdUseCase> _mockGetJigsawCustomerByIdUseCase;
        private Mock<IGetCouncilTaxAccountByAccountRefUseCase> _mockGetCouncilTaxAccountByAccountRefUseCase;
        private GetCustomerByIdUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockCustomerGateway = new Mock<ICustomerGateway>();
            _mockGetPersonApiByIdUseCase = new Mock<IGetPersonApiByIdUseCase>();
            _mockGetJigsawCustomerByIdUseCase = new Mock<IGetJigsawCustomerByIdUseCase>();
            _mockGetCouncilTaxAccountByAccountRefUseCase = new Mock<IGetCouncilTaxAccountByAccountRefUseCase>();
            _classUnderTest = new GetCustomerByIdUseCase(_mockCustomerGateway.Object, _mockGetPersonApiByIdUseCase.Object, _mockGetJigsawCustomerByIdUseCase.Object, _mockGetCouncilTaxAccountByAccountRefUseCase.Object);
            _fixture = new Fixture();

        }

        [Test]
        public void GetsACustomerFromPersonApiJigsawAndAcademyCouncilTax()
        {
            var id = new Guid();
            var userToken = _fixture.Create<string>();
            var redisId = _fixture.Create<string>();
            var mockFirstName = "Luna";
            var mockLastName = "Kitty";
            var mockPersonApi = new Guid().ToString();
            var mockPersonApiName = "PersonAPI";
            var mockJigsawId = "1234";
            var mockJigsawName = "Jigsaw";
            var mockAcademyId = _fixture.Create<string>();
            var mockAcademyName = "AcademyCouncilTax";
            var mockCouncilTaxAccount = _fixture.Create<CouncilTaxAccountInfo>();
            var mockCouncilTaxCustomer =
                _fixture.Build<Customer>().With(x => x.CouncilTaxAccount, mockCouncilTaxAccount);



            _mockCustomerGateway.Setup(x => x.Find(id)).Returns(new SavedCustomer()
            {
                FirstName = mockFirstName,
                LastName = mockLastName,
                DataSources = new List<CustomerDataSource>()
                {
                    new CustomerDataSource()
                    {
                        CustomerId = id, DataSourceId = 2, SourceId = mockJigsawId
                    }, new CustomerDataSource()
                    {
                        CustomerId = id, DataSourceId = 1, SourceId = mockPersonApi
                    }, new CustomerDataSource()
                    {
                        CustomerId = id, DataSourceId = 3, SourceId = mockAcademyId
                    }
                }
            });

            var fakeContactDetails = _fixture.Create<ContactDetails>();
            var fakeKnownAddresses = _fixture.CreateMany<KnownAddress>().ToList();

            var peronsApiCustomer = new Customer()
            {
                Id = mockPersonApi,
                Title = Hackney.Shared.Person.Domain.Title.Miss,
                FirstName = mockFirstName,
                Surname = mockLastName,
                ContactDetails = fakeContactDetails,
                DataSource = new DataSource() { Id = 1, Name = mockPersonApiName },
                DateOfBirth = _fixture.Create<DateTime>(),
                DateOfDeath = null,
                IsAMinor = false,
                KnownAddresses = fakeKnownAddresses,
                NhsNumber = null,
                NiNo = null,
                PreferredSurname = mockLastName,
                PreferredTitle = Hackney.Shared.Person.Domain.Title.Miss,
                PlaceOfBirth = null,
                PreferredFirstName = mockFirstName,
                PreferredMiddleName = null,
                PersonTypes = new List<PersonType> { Hackney.Shared.Person.Domain.PersonType.Tenant }
            };

            _mockGetPersonApiByIdUseCase.Setup(x => x.Execute(mockPersonApi, userToken)).ReturnsAsync(new CustomerResponseObject()
            {
                Customer = peronsApiCustomer,
                SystemIds = new List<SystemId>
                {
                    new SystemId()
                    {
                        Id = mockPersonApi,
                        SystemName = mockPersonApiName
                    }
                }
            });

            var fakeJigsawContactDetails = _fixture.Create<ContactDetails>();
            var fakeJigsawKnownAddresses = _fixture.CreateMany<KnownAddress>().ToList();

            var jigsawApiCustomer = new Customer()
            {
                Id = mockJigsawId,
                Title = null,
                FirstName = mockFirstName,
                Surname = mockLastName,
                ContactDetails = fakeJigsawContactDetails,
                DataSource = new DataSource() { Id = 2, Name = mockJigsawName },
                DateOfBirth = null,
                DateOfDeath = null,
                IsAMinor = false,
                KnownAddresses = fakeJigsawKnownAddresses,
                NhsNumber = _fixture.Create<string>(),
                NiNo = null,
                PreferredSurname = null,
                PreferredTitle = null,
                PlaceOfBirth = null,
                PreferredFirstName = null,
                PreferredMiddleName = null,
                PersonTypes = null
            };

            _mockGetJigsawCustomerByIdUseCase.Setup(x => x.Execute(mockJigsawId, redisId, userToken)).ReturnsAsync(new CustomerResponseObject()
            {
                Customer = jigsawApiCustomer,
                SystemIds = new List<SystemId>
                {
                    new SystemId()
                    {
                        Id = mockJigsawId,
                        SystemName = mockJigsawName
                    }
                }
            });

            var councilTaxAccount = new Customer()
            {
                Id = mockJigsawId,
                Title = null,
                FirstName = mockFirstName,
                Surname = mockLastName,
                ContactDetails = fakeContactDetails,
                DataSource = new DataSource() { Id = 3, Name = mockAcademyName },
                DateOfBirth = null,
                DateOfDeath = null,
                IsAMinor = false,
                KnownAddresses = fakeKnownAddresses,
                NhsNumber = _fixture.Create<string>(),
                NiNo = null,
                PreferredSurname = null,
                PreferredTitle = null,
                PlaceOfBirth = null,
                PreferredFirstName = null,
                PreferredMiddleName = null,
                PersonTypes = null,
                CouncilTaxAccount = mockCouncilTaxAccount
            };



            // _mockGetCouncilTaxAccountByAccountRefUseCase.Setup(x => x.Execute(mockAcademyId, userToken))
            //     .ReturnsAsync(new CustomerResponseObject()
            //     {
            //         Customer = councilTaxAccount,
            //         SystemIds = new List<SystemId>
            //         {
            //             new SystemId()
            //             {
            //                 Id = mockAcademyId,
            //                 SystemName = mockAcademyName
            //             }
            //         }
            //     });

            var result = _classUnderTest.Execute(id, userToken, redisId);

            result.SystemIds[0].SystemName.Should().BeEquivalentTo(mockJigsawName);
            result.SystemIds[0].Id.Should().BeEquivalentTo(mockJigsawId);
            result.SystemIds[1].SystemName.Should().BeEquivalentTo(mockPersonApiName);
            result.SystemIds[1].Id.Should().BeEquivalentTo(mockPersonApi);
            //            result.SystemIds[2].SystemName.Should().BeEquivalentTo(mockAcademyName);
            //            result.SystemIds[2].Id.Should().BeEquivalentTo(mockAcademyId);

            result.Customer.Id.Should().BeEquivalentTo(id.ToString());
            result.Customer.Title.Should().BeEquivalentTo(peronsApiCustomer.Title);
            result.Customer.FirstName.Should().BeEquivalentTo(mockFirstName);
            result.Customer.Surname.Should().BeEquivalentTo(mockLastName);
            result.Customer.ContactDetails.Count.Should().Equals(2);
            result.Customer.DateOfBirth.Should().Equals(peronsApiCustomer.DateOfBirth);
            result.Customer.DateOfDeath.Should().Equals(null);
            result.Customer.IsAMinor.Should().Equals(peronsApiCustomer.IsAMinor);
            result.Customer.KnownAddresses.Count.Should().Equals(fakeKnownAddresses.Count + fakeJigsawKnownAddresses.Count);
            result.Customer.NhsNumber.Should().BeEquivalentTo(jigsawApiCustomer.NhsNumber);
            result.Customer.NiNo.Should().BeEquivalentTo(null);
            result.Customer.PreferredSurname.Should().BeEquivalentTo(peronsApiCustomer.PreferredSurname);
            result.Customer.PreferredTitle.Should().BeEquivalentTo(peronsApiCustomer.PreferredTitle);
            result.Customer.PlaceOfBirth.Should().BeEquivalentTo(peronsApiCustomer.PlaceOfBirth);
            result.Customer.PreferredFirstName.Should().BeEquivalentTo(peronsApiCustomer.PreferredFirstName);
            result.Customer.PreferredMiddleName.Should().BeEquivalentTo(null);
            result.Customer.PersonTypes.Should().BeEquivalentTo(peronsApiCustomer.PersonTypes);
            //result.Customer.CouncilTaxAccount.Should().BeEquivalentTo(mockCouncilTaxAccount);
        }

        [Test]
        public void GetsACustomerFromPersonApi()
        {
            var id = new Guid();
            var userToken = _fixture.Create<string>();
            string redisId = null;
            var mockFirstName = "Luna";
            var mockLastName = "Kitty";
            var mockPersonApi = new Guid().ToString();
            var mockPersonApiName = "PersonAPI";
            var mockJigsawId = "1234";
            var mockJigsawName = "Jigsaw";


            _mockCustomerGateway.Setup(x => x.Find(id)).Returns(new SavedCustomer()
            {
                FirstName = mockFirstName,
                LastName = mockLastName,
                DataSources = new List<CustomerDataSource>()
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

            var fakeContactDetails = _fixture.Create<ContactDetails>();
            var fakeKnownAddresses = _fixture.CreateMany<KnownAddress>().ToList();

            var peronsApiCustomer = new Customer()
            {
                Id = mockPersonApi,
                Title = Hackney.Shared.Person.Domain.Title.Miss,
                FirstName = mockFirstName,
                Surname = mockLastName,
                ContactDetails = fakeContactDetails,
                DataSource = new DataSource() { Id = 1, Name = mockPersonApiName },
                DateOfBirth = _fixture.Create<DateTime>(),
                DateOfDeath = null,
                IsAMinor = false,
                KnownAddresses = fakeKnownAddresses,
                NhsNumber = null,
                NiNo = null,
                PreferredSurname = mockLastName,
                PreferredTitle = Hackney.Shared.Person.Domain.Title.Miss,
                PlaceOfBirth = null,
                PreferredFirstName = mockFirstName,
                PreferredMiddleName = null,
                PersonTypes = new List<PersonType> { Hackney.Shared.Person.Domain.PersonType.Tenant }
            };

            _mockGetPersonApiByIdUseCase.Setup(x => x.Execute(mockPersonApi, userToken)).ReturnsAsync(new CustomerResponseObject()
            {
                Customer = peronsApiCustomer,
                SystemIds = new List<SystemId>
                {
                    new SystemId()
                    {
                        Id = mockPersonApi,
                        SystemName = mockPersonApiName
                    }
                }
            });

            var result = _classUnderTest.Execute(id, userToken, redisId);

            result.SystemIds[0].SystemName.Should().BeEquivalentTo(mockJigsawName);
            result.SystemIds[0].Id.Should().BeEquivalentTo(mockJigsawId);
            result.SystemIds[0].Error.Should().BeEquivalentTo("Unauthorised");
            result.SystemIds[1].SystemName.Should().BeEquivalentTo(mockPersonApiName);
            result.SystemIds[1].Id.Should().BeEquivalentTo(mockPersonApi);

            result.Customer.Id.Should().BeEquivalentTo(id.ToString());
            result.Customer.Title.Should().BeEquivalentTo(peronsApiCustomer.Title);
            result.Customer.FirstName.Should().BeEquivalentTo(mockFirstName);
            result.Customer.Surname.Should().BeEquivalentTo(mockLastName);
            result.Customer.ContactDetails.Count.Should().Equals(2);
            result.Customer.DateOfBirth.Should().Equals(peronsApiCustomer.DateOfBirth);
            result.Customer.DateOfDeath.Should().Equals(null);
            result.Customer.IsAMinor.Should().Equals(peronsApiCustomer.IsAMinor);
            result.Customer.KnownAddresses.Should().Equals(fakeKnownAddresses);
            result.Customer.NhsNumber.Should().BeEquivalentTo(null);
            result.Customer.NiNo.Should().BeEquivalentTo(null);
            result.Customer.PreferredSurname.Should().BeEquivalentTo(peronsApiCustomer.PreferredSurname);
            result.Customer.PreferredTitle.Should().BeEquivalentTo(peronsApiCustomer.PreferredTitle);
            result.Customer.PlaceOfBirth.Should().BeEquivalentTo(peronsApiCustomer.PlaceOfBirth);
            result.Customer.PreferredFirstName.Should().BeEquivalentTo(peronsApiCustomer.PreferredFirstName);
            result.Customer.PreferredMiddleName.Should().BeEquivalentTo(null);
            result.Customer.PersonTypes.Should().BeEquivalentTo(peronsApiCustomer.PersonTypes);

            _mockGetJigsawCustomerByIdUseCase.Verify(x => x.Execute(mockJigsawId, redisId, userToken), Times.Never);

        }
    }
}
