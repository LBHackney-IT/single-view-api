using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase;

public class GetHousingBenefitsAccountsByCustomerNameUseCaseTests : LogCallAspectFixture
{
    private GetHousingBenefitsAccountsByCustomerNameUseCase _classUnderTest;
    private Fixture _fixture;
    private Mock<IAcademyGateway> _mockAcademyGateway;
    private Mock<IDataSourceGateway> _mockDataSourceGateway;

    [SetUp]
    public void SetUp()
    {
        _mockAcademyGateway = new Mock<IAcademyGateway>();
        _mockDataSourceGateway = new Mock<IDataSourceGateway>();
        _classUnderTest =
            new GetHousingBenefitsAccountsByCustomerNameUseCase(_mockAcademyGateway.Object,
                _mockDataSourceGateway.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void ExecuteResponseErrorHasNotFoundMessageIfGatewayReturnsNull()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubDataSource = new DataSource { Id = 4, Name = "Academy - Housing Benefits" };

        _mockDataSourceGateway.Setup(x => x.GetEntityById(4)).Returns(stubDataSource);
        _mockAcademyGateway.Setup(x => x.GetHousingBenefitsAccountsByCustomerName(firstName, lastName, userToken))
            .ReturnsAsync((HousingBenefitsSearchResponseObject) null);

        var result = _classUnderTest.Execute(firstName, lastName, userToken).Result;

        Assert.That(result.SystemIds[0].Error, Is.EqualTo(SystemId.NotFoundMessage));
        ;
    }

    [Test]
    public void ExecuteSuccessfulResponseHasSearchResults()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubDataSource = new DataSource { Id = 4, Name = "Academy - Housing Benefits" };
        var stubEntity = _fixture.Build<HousingBenefitsSearchResponseObject>()
            .Without(x => x.Error).Create();

        _mockDataSourceGateway.Setup(x => x.GetEntityById(4)).Returns(stubDataSource);
        _mockAcademyGateway.Setup(x =>
            x.GetHousingBenefitsAccountsByCustomerName(firstName, lastName, userToken)).ReturnsAsync(stubEntity);

        var results = _classUnderTest.Execute(firstName, lastName, userToken).Result;

        results.SearchResponse.UngroupedResults[^1].Should().Equals(stubEntity);
    }
}
