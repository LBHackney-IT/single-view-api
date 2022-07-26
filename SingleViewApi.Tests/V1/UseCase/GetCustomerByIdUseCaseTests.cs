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
using Microsoft.OpenApi.Extensions;
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
        private Mock<IGetHousingBenefitsAccountByAccountRefUseCase> _mockGetHousingBenefitsAccountByAccountRefUseCase;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockCustomerGateway = new Mock<ICustomerGateway>();
            _mockGetPersonApiByIdUseCase = new Mock<IGetPersonApiByIdUseCase>();
            _mockGetJigsawCustomerByIdUseCase = new Mock<IGetJigsawCustomerByIdUseCase>();
            _mockGetCouncilTaxAccountByAccountRefUseCase = new Mock<IGetCouncilTaxAccountByAccountRefUseCase>();
            _mockGetHousingBenefitsAccountByAccountRefUseCase = new Mock<IGetHousingBenefitsAccountByAccountRefUseCase>();
            _classUnderTest = new GetCustomerByIdUseCase(
                _mockCustomerGateway.Object,
                _mockGetPersonApiByIdUseCase.Object,
                _mockGetJigsawCustomerByIdUseCase.Object,
                _mockGetCouncilTaxAccountByAccountRefUseCase.Object,
                _mockGetHousingBenefitsAccountByAccountRefUseCase.Object);
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
            var mockCouncilTaxId = _fixture.Create<string>();
            var mockCouncilTaxName = "AcademyCouncilTax";
            var mockHousingBenefitsId = _fixture.Create<string>();
            var mockHousingBenefitsName = "AcademyHousingBenefits";
            var mockCouncilTaxAccount = _fixture.Create<CouncilTaxAccountInfo>();
            var mockDateOfBirth = _fixture.Create<DateTime>();

            _mockCustomerGateway.Setup(x => x.Find(id)).Returns(new SavedCustomer()
            {
                FirstName = mockFirstName,
                LastName = mockLastName,
                DateOfBirth = mockDateOfBirth,
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
                        CustomerId = id, DataSourceId = 3, SourceId = mockCouncilTaxId
                    }, new CustomerDataSource()
                    {
                        CustomerId = id, DataSourceId = 4, SourceId = mockHousingBenefitsId
                    }
                }
            });

            var fakeContactDetails = new List<ContactDetails>() { _fixture.Create<ContactDetails>() };
            var fakeKnownAddresses = _fixture.CreateMany<KnownAddress>().ToList();
            var fakeCautionaryAlerts = _fixture.CreateMany<CautionaryAlert>().ToList();

            var peronsApiCustomer = new Customer()
            {
                Id = mockPersonApi,
                Title = Hackney.Shared.Person.Domain.Title.Miss.GetDisplayName(),
                FirstName = mockFirstName,
                Surname = mockLastName,
                ContactDetails = fakeContactDetails,
                DataSource = new DataSource() { Id = 1, Name = mockPersonApiName },
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
                PersonTypes = new List<string> { Hackney.Shared.Person.Domain.PersonType.Tenant.ToDescription() },
                CautionaryAlerts = fakeCautionaryAlerts
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

            var fakeJigsawContactDetails = new List<ContactDetails>() { _fixture.Create<ContactDetails>() };
            var fakeJigsawKnownAddresses = _fixture.CreateMany<KnownAddress>().ToList();
            var fakeJigsawCautionaryAlerts = _fixture.CreateMany<CautionaryAlert>().ToList();

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
                PersonTypes = null,
                CautionaryAlerts = fakeCautionaryAlerts
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
                DataSource = new DataSource() { Id = 3, Name = mockCouncilTaxName },
                DateOfBirth = null,
                DateOfDeath = null,
                IsAMinor = false,
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

            _mockGetCouncilTaxAccountByAccountRefUseCase.Setup(x => x.Execute(mockCouncilTaxId, userToken))
                .ReturnsAsync(new CustomerResponseObject()
                {
                    Customer = councilTaxAccount,
                    SystemIds = new List<SystemId>
                    {
                        new SystemId()
                        {
                            Id = mockCouncilTaxId,
                            SystemName = mockCouncilTaxName
                        }
                    }
                });

            var mockHousingBenefitsAccount = _fixture.Create<HousingBenefitsAccountInfo>();

            var housingBenefitsCustomer = new Customer()
            {
                Id = mockHousingBenefitsId,
                Title = null,
                FirstName = mockFirstName,
                Surname = mockLastName,
                DataSource = new DataSource() { Id = 4, Name = mockHousingBenefitsName },
                HousingBenefitsAccount = mockHousingBenefitsAccount
            };

            _mockGetHousingBenefitsAccountByAccountRefUseCase.Setup(x => x.Execute(mockHousingBenefitsId, userToken))
                .ReturnsAsync(new CustomerResponseObject()
                {
                    Customer = housingBenefitsCustomer,
                    SystemIds = new List<SystemId>
                    {
                        new SystemId()
                        {
                            Id = mockCouncilTaxId,
                            SystemName = mockCouncilTaxName
                        }
                    }
                });

            var result = _classUnderTest.Execute(id, userToken, redisId);

            result.SystemIds[0].SystemName.Should().BeEquivalentTo(mockJigsawName);
            result.SystemIds[0].Id.Should().BeEquivalentTo(mockJigsawId);
            result.SystemIds[1].SystemName.Should().BeEquivalentTo(mockPersonApiName);
            result.SystemIds[1].Id.Should().BeEquivalentTo(mockPersonApi);
            result.SystemIds[2].SystemName.Should().BeEquivalentTo(mockCouncilTaxName);
            result.SystemIds[2].Id.Should().BeEquivalentTo(mockCouncilTaxId);
            result.Customer.Id.Should().BeEquivalentTo(id.ToString());
            result.Customer.Title.Should().BeEquivalentTo(peronsApiCustomer.Title);
            result.Customer.FirstName.Should().BeEquivalentTo(mockFirstName);
            result.Customer.Surname.Should().BeEquivalentTo(mockLastName);
            result.Customer.AllContactDetails.Count.Should().Be(2);

            result.Customer.AllContactDetails[0].DataSourceName.Should().BeEquivalentTo(mockJigsawName);
            result.Customer.AllContactDetails[0].IsActive.Should().Be(fakeJigsawContactDetails[0].IsActive);
            result.Customer.AllContactDetails[0].SourceServiceArea.Should().BeEquivalentTo(fakeJigsawContactDetails[0].SourceServiceArea.Area);
            result.Customer.AllContactDetails[0].ContactType.Should().BeEquivalentTo(fakeJigsawContactDetails[0].ContactInformation.ContactType.ToString());
            result.Customer.AllContactDetails[0].SubType.Should().BeEquivalentTo(fakeJigsawContactDetails[0].ContactInformation.SubType.ToString());
            result.Customer.AllContactDetails[0].Value.Should().BeEquivalentTo(fakeJigsawContactDetails[0].ContactInformation.Value);
            result.Customer.AllContactDetails[0].Description.Should().BeEquivalentTo(fakeJigsawContactDetails[0].ContactInformation.Description);
            result.Customer.AllContactDetails[0].AddressExtended.Should().BeEquivalentTo(fakeJigsawContactDetails[0].ContactInformation.AddressExtended);

            result.Customer.AllContactDetails[1].DataSourceName.Should().BeEquivalentTo(mockPersonApiName);
            result.Customer.AllContactDetails[1].IsActive.Should().Be(fakeContactDetails[0].IsActive);
            result.Customer.AllContactDetails[1].SourceServiceArea.Should().BeEquivalentTo(fakeContactDetails[0].SourceServiceArea.Area);
            result.Customer.AllContactDetails[1].ContactType.Should().BeEquivalentTo(fakeContactDetails[0].ContactInformation.ContactType.ToString());
            result.Customer.AllContactDetails[1].SubType.Should().BeEquivalentTo(fakeContactDetails[0].ContactInformation.SubType.ToString());
            result.Customer.AllContactDetails[1].Value.Should().BeEquivalentTo(fakeContactDetails[0].ContactInformation.Value);
            result.Customer.AllContactDetails[1].Description.Should().BeEquivalentTo(fakeContactDetails[0].ContactInformation.Description);
            result.Customer.AllContactDetails[1].AddressExtended.Should().BeEquivalentTo(fakeContactDetails[0].ContactInformation.AddressExtended);


            result.Customer.DateOfBirth.Should().Be(mockDateOfBirth);
            result.Customer.DateOfDeath.Should().BeNull();
            result.Customer.IsAMinor.Should().Be(peronsApiCustomer.IsAMinor);
            result.Customer.KnownAddresses.Count.Should().Be(fakeKnownAddresses.Count + fakeJigsawKnownAddresses.Count);
            result.Customer.NhsNumber.Should().BeEquivalentTo(jigsawApiCustomer.NhsNumber);
            result.Customer.NiNo.Should().BeNull();
            result.Customer.PreferredSurname.Should().BeEquivalentTo(peronsApiCustomer.PreferredSurname);
            result.Customer.PreferredTitle.Should().BeEquivalentTo(peronsApiCustomer.PreferredTitle);
            result.Customer.PlaceOfBirth.Should().BeEquivalentTo(peronsApiCustomer.PlaceOfBirth);
            result.Customer.PreferredFirstName.Should().BeEquivalentTo(peronsApiCustomer.PreferredFirstName);
            result.Customer.PreferredMiddleName.Should().BeEquivalentTo(null);
            result.Customer.PersonTypes.Should().BeEquivalentTo(peronsApiCustomer.PersonTypes);
            result.Customer.CouncilTaxAccount.Should().BeEquivalentTo(mockCouncilTaxAccount);
            result.Customer.HousingBenefitsAccount.Should().BeEquivalentTo(mockHousingBenefitsAccount);
            result.Customer.CautionaryAlerts.Count.Should().Be(fakeCautionaryAlerts.Count + fakeJigsawCautionaryAlerts.Count);
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
            var mockDateOfBirth = _fixture.Create<DateTime>();


            _mockCustomerGateway.Setup(x => x.Find(id)).Returns(new SavedCustomer()
            {
                FirstName = mockFirstName,
                LastName = mockLastName,
                DateOfBirth = mockDateOfBirth,
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

            var fakeContactDetails = new List<ContactDetails>(){new ContactDetails()
            {
                ContactInformation = new ContactInformation()
                {
                    Value = "something not useful"
                }
            }};
            var fakeKnownAddresses = _fixture.CreateMany<KnownAddress>().ToList();
            var fakeCautionaryAlerts = _fixture.CreateMany<CautionaryAlert>().ToList();

            var peronsApiCustomer = new Customer()
            {
                Id = mockPersonApi,
                Title = Hackney.Shared.Person.Domain.Title.Miss.GetDisplayName(),
                FirstName = mockFirstName,
                Surname = mockLastName,
                ContactDetails = fakeContactDetails,
                DataSource = new DataSource() { Id = 1, Name = mockPersonApiName },
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
                PersonTypes = new List<string> { Hackney.Shared.Person.Domain.PersonType.Tenant.ToDescription() },
                CautionaryAlerts = fakeCautionaryAlerts
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

            result.Customer.AllContactDetails.Count.Should().Be(1);
            result.Customer.AllContactDetails[0].DataSourceName.Should().BeEquivalentTo(mockPersonApiName);
            result.Customer.AllContactDetails[0].IsActive.Should().Be(fakeContactDetails[0].IsActive);
            result.Customer.AllContactDetails[0].ContactType.Should().BeEquivalentTo(fakeContactDetails[0].ContactInformation.ContactType.ToString());
            result.Customer.AllContactDetails[0].SubType.Should().BeEquivalentTo(fakeContactDetails[0].ContactInformation.SubType.ToString());
            result.Customer.AllContactDetails[0].Value.Should().BeEquivalentTo(fakeContactDetails[0].ContactInformation.Value);
            result.Customer.AllContactDetails[0].Description.Should().BeEquivalentTo(fakeContactDetails[0].ContactInformation.Description);
            result.Customer.AllContactDetails[0].AddressExtended.Should().BeEquivalentTo(fakeContactDetails[0].ContactInformation.AddressExtended);

            result.Customer.DateOfBirth.Should().Be(mockDateOfBirth);
            result.Customer.DateOfDeath.Should().Be(null);
            result.Customer.IsAMinor.Should().Be(peronsApiCustomer.IsAMinor);
            result.Customer.KnownAddresses.Should().Equal(fakeKnownAddresses);
            result.Customer.NhsNumber.Should().BeEquivalentTo(null);
            result.Customer.NiNo.Should().BeEquivalentTo(null);
            result.Customer.PreferredSurname.Should().BeEquivalentTo(peronsApiCustomer.PreferredSurname);
            result.Customer.PreferredTitle.Should().BeEquivalentTo(peronsApiCustomer.PreferredTitle);
            result.Customer.PlaceOfBirth.Should().BeEquivalentTo(peronsApiCustomer.PlaceOfBirth);
            result.Customer.PreferredFirstName.Should().BeEquivalentTo(peronsApiCustomer.PreferredFirstName);
            result.Customer.PreferredMiddleName.Should().BeEquivalentTo(null);
            result.Customer.PersonTypes.Should().BeEquivalentTo(peronsApiCustomer.PersonTypes);
            result.Customer.CautionaryAlerts.Should().Equal(fakeCautionaryAlerts);

            _mockGetJigsawCustomerByIdUseCase.Verify(x => x.Execute(mockJigsawId, redisId, userToken), Times.Never);
        }
    }
}
