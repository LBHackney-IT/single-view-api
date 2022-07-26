using System.Collections.Generic;
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
public class GetJigsawCustomersUseCaseTests
{
    private Mock<IJigsawGateway> _mockJigsawGateway;
    private Mock<IGetJigsawAuthTokenUseCase> _mockGetJigsawAuthTokenUseCase;
    private GetJigsawCustomersUseCase _classUnderTest;
    private Fixture _fixture;
    private Mock<IDataSourceGateway> _mockDataSourceGateway;

    [SetUp]
    public void SetUp()
    {
        _mockJigsawGateway = new Mock<IJigsawGateway>();
        _mockGetJigsawAuthTokenUseCase = new Mock<IGetJigsawAuthTokenUseCase>();
        _mockDataSourceGateway = new Mock<IDataSourceGateway>();
        _classUnderTest = new GetJigsawCustomersUseCase(_mockJigsawGateway.Object, _mockGetJigsawAuthTokenUseCase.Object, _mockDataSourceGateway.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void UseCaseReturnsErrorWhenThereIsNoJigsawToken()
    {
        var redisId = _fixture.Create<string>();
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        const string hackneyToken = "test-token";
        var stubbedJigsawDataSource = _fixture.Create<DataSource>();

        _mockDataSourceGateway.Setup(x => x.GetEntityById(2)).Returns(stubbedJigsawDataSource);
        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken)).ReturnsAsync(new AuthGatewayResponse() { Token = null, ExceptionMessage = "No token present" }); ;

        var result = _classUnderTest.Execute(firstName, lastName, redisId, hackneyToken).Result;

        Assert.That(result.SystemIds[0].Error, Is.EqualTo("No token present")); ;

    }

    [Test]
    public void ReturnsSearchResultsFromJigsaw()
    {
        var firstName = _fixture.Create<string>();
        var redisId = _fixture.Create<string>();
        var jigsawToken = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        const string hackneyToken = "test-token";
        var searchText = $"{firstName}+{lastName}";
        var stubbedEntity = _fixture.Create<List<JigsawCustomerSearchApiResponseObject>>();
        var stubbedDataSource = _fixture.Create<DataSource>();

        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId, hackneyToken)).ReturnsAsync(new AuthGatewayResponse() { Token = jigsawToken, ExceptionMessage = null });
        _mockDataSourceGateway.Setup(x => x.GetEntityById(2)).Returns(stubbedDataSource);

        _mockJigsawGateway.Setup(x => x.GetCustomers(firstName, lastName, jigsawToken)).ReturnsAsync(stubbedEntity);

        var results = _classUnderTest.Execute(firstName, lastName, redisId, hackneyToken).Result;

        results.SystemIds[^1].SystemName.Should().BeEquivalentTo(stubbedDataSource.Name);
        results.SystemIds[^1].Id.Should().BeEquivalentTo(searchText);
        results.SearchResponse.UngroupedResults[0].FirstName.Should().BeEquivalentTo(stubbedEntity[0].FirstName);
        results.SearchResponse.UngroupedResults[0].SurName.Should().BeEquivalentTo(stubbedEntity[0].LastName);
        results.SearchResponse.UngroupedResults[0].DateOfBirth.Should().Be(stubbedEntity[0].DoB);
        results.SearchResponse.UngroupedResults[0].DataSource.Should().BeEquivalentTo(stubbedDataSource.Name);
    }
}
