using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase;

public class GetJigsawCasesByCustomerIdUseCaseTests : LogCallAspectFixture
{
    private Mock<IJigsawGateway> _mockJigsawGateway;
    private Mock<IGetJigsawAuthTokenUseCase> _mockGetJigsawAuthTokenUseCase;
    private GetJigsawCasesByCustomerIdUseCase _classUnderTest;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _mockJigsawGateway = new Mock<IJigsawGateway>();
        _mockGetJigsawAuthTokenUseCase = new Mock<IGetJigsawAuthTokenUseCase>();
        _classUnderTest =
            new GetJigsawCasesByCustomerIdUseCase(_mockJigsawGateway.Object, _mockGetJigsawAuthTokenUseCase.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void ReturnsNullIfThereIsNoJigsawToken()
    {
        var redisId = _fixture.Create<string>();
        var customerId = _fixture.Create<string>();
        var hackneyToken = _fixture.Create<string>();

        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken))
            .ReturnsAsync(new AuthGatewayResponse() { Token = null, ExceptionMessage = "No token present" });

        var result = _classUnderTest.Execute(customerId, redisId, hackneyToken).Result;

        Assert.IsNull(result);
    }

    [Test]
    public void ReturnsACaseResponseObject()
    {
        var redisId = _fixture.Create<string>();
        var customerId = _fixture.Create<string>();
        var hackneyToken = _fixture.Create<string>();
        var jigsawToken = _fixture.Create<string>();
        var mockCustomerCases = _fixture.Create<JigsawCasesResponseObject>();
        var mockCurrentCustomerCaseId = mockCustomerCases.Cases[0].Id;
        var mockCustomerCaseOverviews = _fixture.Build<JigsawCaseOverviewResponseObject>()
            .With(x => x.CustomerId, mockCurrentCustomerCaseId).Create();
        var mockCustomerPlacements = _fixture.Create<JigsawCasePlacementInformationResponseObject>();

        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken)).ReturnsAsync(new AuthGatewayResponse() { Token = jigsawToken, ExceptionMessage = null });
        _mockJigsawGateway.Setup(x => x.GetCasesByCustomerId(customerId, jigsawToken)).ReturnsAsync(mockCustomerCases);
        _mockJigsawGateway.Setup(x => x.GetCaseOverviewByCaseId(mockCurrentCustomerCaseId.ToString(), jigsawToken))
            .ReturnsAsync(mockCustomerCaseOverviews);
        _mockJigsawGateway.Setup(x => x.GetCaseAccommodationPlacementsByCaseId(mockCurrentCustomerCaseId.ToString(), jigsawToken))
            .ReturnsAsync(mockCustomerPlacements);

        var result = _classUnderTest.Execute(customerId, redisId, hackneyToken).Result;

        result.Cases[0].Should().BeEquivalentTo(mockCustomerCases.Cases[0]);
        result.CaseOverviews[0].Id.Should().BeEquivalentTo(mockCustomerCaseOverviews.Id.ToString());
        result.CaseOverviews[0].HouseHoldComposition.Should().BeEquivalentTo(mockCustomerCaseOverviews.HouseholdComposition);
        result.PlacementInformation[0].DclgClassificationType.Should()
            .BeEquivalentTo(mockCustomerPlacements.Placements[0].DclgClassificationType);

    }
}
