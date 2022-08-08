using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase;

[TestFixture]
public class GetJigsawCustomerByIdUseCaseTest
{
    private Mock<IJigsawGateway> _mockJigsawGateway;
    private Mock<IGetJigsawAuthTokenUseCase> _mockGetJigsawAuthTokenUseCase;
    private GetJigsawCustomerByIdUseCase _classUnderTest;
    private Fixture _fixture;
    private Mock<IDataSourceGateway> _mockDataSourceGateway;

    [SetUp]
    public void SetUp()
    {
        _mockJigsawGateway = new Mock<IJigsawGateway>();
        _mockGetJigsawAuthTokenUseCase = new Mock<IGetJigsawAuthTokenUseCase>();
        _mockDataSourceGateway = new Mock<IDataSourceGateway>();
        _classUnderTest = new GetJigsawCustomerByIdUseCase(_mockJigsawGateway.Object, _mockGetJigsawAuthTokenUseCase.Object, _mockDataSourceGateway.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void UseCaseReturnsNullIfAuthFails()
    {
        string redisId = _fixture.Create<string>();
        const string hackneyToken = "test-token";
        string stubbedCustomerId = _fixture.Create<string>();

        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken)).ReturnsAsync(new AuthGatewayResponse() { Token = null, ExceptionMessage = "No token present" }); ;

        var result = _classUnderTest.Execute(stubbedCustomerId, redisId, hackneyToken).Result;

        Assert.That(result, Is.EqualTo(null));

    }

    [Test]
    public void ReturnsCustomerResponseObject()
    {
        var redisId = _fixture.Create<string>();
        var jigsawToken = _fixture.Create<string>();
        const string hackneyToken = "test-token";
        const string stubbedCorrespondenceAddress = "123 Some St, E8 3YD";
        const string stubbedEmailAddress = "some@example.com";
        const string stubbedHomePhoneNumber = "020 123 1234";
        const string stubbedMobilePhoneNumber = "070 123 1234";
        const string stubbedWorkPhoneNumber = "070 321 4321";
        const string stubbedPregnancyDueDate = "2022-10-01T00:00:00Z";
        const string stubbedAccommodationTypeId = "111";
        const string stubbedHousingCircumstanceId = "112";
        const bool stubbedIsSettled = true;
        SupportWorker stubbedSupportWorker = _fixture.Create<SupportWorker>();

        var stubbedEntity = _fixture.Create<JigsawCustomerResponseObject>();

        stubbedEntity.PersonInfo.OkToContactOnEmail = true;
        stubbedEntity.PersonInfo.EmailAddress = stubbedEmailAddress;
        stubbedEntity.PersonInfo.OkToContactOnHomePhoneNumber = true;
        stubbedEntity.PersonInfo.HomePhoneNumber = stubbedHomePhoneNumber;
        stubbedEntity.PersonInfo.OkToContactOnMobilePhoneNumber = true;
        stubbedEntity.PersonInfo.MobilePhoneNumber = stubbedMobilePhoneNumber;
        stubbedEntity.PersonInfo.OkToContactOnWorkPhoneNumber = true;
        stubbedEntity.PersonInfo.WorkPhoneNumber = stubbedWorkPhoneNumber;
        stubbedEntity.PersonInfo.CorrespondenceAddressString = stubbedCorrespondenceAddress;
        stubbedEntity.PersonInfo.PregnancyDueDate = stubbedPregnancyDueDate;
        stubbedEntity.PersonInfo.AccommodationTypeId = stubbedAccommodationTypeId;
        stubbedEntity.PersonInfo.HousingCircumstanceId = stubbedHousingCircumstanceId;
        stubbedEntity.PersonInfo.IsSettled = stubbedIsSettled;
        stubbedEntity.PersonInfo.SupportWorker = stubbedSupportWorker;

        var stubbedDataSource = _fixture.Create<DataSource>();
        var stubbedCustomerId = _fixture.Create<string>();

        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken)).ReturnsAsync(new AuthGatewayResponse() { Token = jigsawToken, ExceptionMessage = null });
        _mockDataSourceGateway.Setup(x => x.GetEntityById(2)).Returns(stubbedDataSource);

        _mockJigsawGateway.Setup(x => x.GetCustomerById(stubbedCustomerId, jigsawToken)).ReturnsAsync(stubbedEntity);

        var results = _classUnderTest.Execute(stubbedCustomerId, redisId, hackneyToken).Result;

        results.SystemIds[^1].SystemName.Should().BeEquivalentTo(stubbedDataSource.Name);
        results.SystemIds[^1].Id.Should().BeEquivalentTo(stubbedEntity.Id);
        results.Customer.FirstName.Should().BeEquivalentTo(stubbedEntity.PersonInfo.FirstName);
        results.Customer.Surname.Should().BeEquivalentTo(stubbedEntity.PersonInfo.LastName);
        results.Customer.DateOfBirth.Should().Be(stubbedEntity.PersonInfo.DateOfBirth);
        results.Customer.DataSource.Should().Be(stubbedDataSource);
        results.Customer.NiNo.Should().BeEquivalentTo(stubbedEntity.PersonInfo.NationalInsuranceNumber);
        results.Customer.NhsNumber.Should().BeEquivalentTo(stubbedEntity.PersonInfo.NhsNumber);
        results.Customer.KnownAddresses[0].FullAddress.Should().BeEquivalentTo(stubbedEntity.PersonInfo.AddressString);
        results.Customer.KnownAddresses[0].CurrentAddress.Should().Be(true);
        results.Customer.KnownAddresses[0].DataSourceName.Should().BeEquivalentTo(stubbedDataSource.Name);
        results.Customer.PregnancyDueDate.Should().Be(stubbedEntity.PersonInfo.PregnancyDueDate);
        results.Customer.AccommodationTypeId.Should().Be(stubbedEntity.PersonInfo.AccommodationTypeId);
        results.Customer.HousingCircumstanceId.Should().Be(stubbedEntity.PersonInfo.HousingCircumstanceId);
        results.Customer.IsSettled.Should().BeTrue();
        results.Customer.SupportWorker.Should().Be(stubbedEntity.PersonInfo.SupportWorker);
        results.Customer.Gender.Should().Be(stubbedEntity.PersonInfo.Gender);

        results.Customer.AllContactDetails[0].ContactType.Should().BeEquivalentTo("Email");
        results.Customer.AllContactDetails[0].DataSourceName.Should().BeEquivalentTo(stubbedDataSource.Name);
        results.Customer.AllContactDetails[0].Value.Should().BeEquivalentTo(stubbedEmailAddress);

        results.Customer.AllContactDetails[1].ContactType.Should().BeEquivalentTo("Phone");
        results.Customer.AllContactDetails[1].SubType.Should().BeEquivalentTo("Home");
        results.Customer.AllContactDetails[1].DataSourceName.Should().BeEquivalentTo(stubbedDataSource.Name);
        results.Customer.AllContactDetails[1].Value.Should().BeEquivalentTo(stubbedHomePhoneNumber);

        results.Customer.AllContactDetails[2].ContactType.Should().BeEquivalentTo("Phone");
        results.Customer.AllContactDetails[2].SubType.Should().BeEquivalentTo("Mobile");
        results.Customer.AllContactDetails[2].DataSourceName.Should().BeEquivalentTo(stubbedDataSource.Name);
        results.Customer.AllContactDetails[2].Value.Should().BeEquivalentTo(stubbedMobilePhoneNumber);

        results.Customer.AllContactDetails[3].ContactType.Should().BeEquivalentTo("Phone");
        results.Customer.AllContactDetails[3].SubType.Should().BeEquivalentTo("Work Phone");
        results.Customer.AllContactDetails[3].DataSourceName.Should().BeEquivalentTo(stubbedDataSource.Name);
        results.Customer.AllContactDetails[3].Value.Should().BeEquivalentTo(stubbedWorkPhoneNumber);
    }

    [Test]
    public void DoesNotReturnInvalidContactDetails()
    {
        var redisId = _fixture.Create<string>();
        var jigsawToken = _fixture.Create<string>();
        const string hackneyToken = "test-token";

        var stubbedEntity = _fixture.Build<JigsawCustomerResponseObject>()
            .With(o => o.PersonInfo, new PersonInfo()
            {
                CorrespondenceAddress = "",
                OkToContactOnEmail = false,
                OkToContactOnHomePhoneNumber = false,
                OkToContactOnMobilePhoneNumber = false,
                OkToContactOnWorkPhoneNumber = false
            }).Create();


        var stubbedDataSource = _fixture.Create<DataSource>();
        var stubbedCustomerId = _fixture.Create<string>();

        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken)).ReturnsAsync(new AuthGatewayResponse() { Token = jigsawToken, ExceptionMessage = null });
        _mockDataSourceGateway.Setup(x => x.GetEntityById(2)).Returns(stubbedDataSource);

        _mockJigsawGateway.Setup(x => x.GetCustomerById(stubbedCustomerId, jigsawToken)).ReturnsAsync(stubbedEntity);

        var results = _classUnderTest.Execute(stubbedCustomerId, redisId, hackneyToken).Result;

        results.Customer.AllContactDetails.Should().BeNull();
    }
}
