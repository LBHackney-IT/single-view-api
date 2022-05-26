using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways;
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

    [SetUp]
    public void SetUp()
    {
        _mockJigsawGateway = new Mock<IJigsawGateway>();
        _mockGetJigsawAuthTokenUseCase = new Mock<IGetJigsawAuthTokenUseCase>();
        _classUnderTest = new GetJigsawCustomersUseCase(_mockJigsawGateway.Object, _mockGetJigsawAuthTokenUseCase.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void UseCaseReturnsNullWhenThereIsNoJigsawToken()
    {
        var redisId = _fixture.Create<string>();
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId)).ReturnsAsync("");

        var result = _classUnderTest.Execute(firstName, lastName, redisId).Result;

        Assert.IsNull(result);

    }

    [Test]
    public void ReturnsSearchResultsFromJigsaw()
    {
        var firstName = _fixture.Create<string>();
        var redisId = _fixture.Create<string>();
        var jigsawToken = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var searchText = $"{firstName}+{lastName}";
        var stubbedEntity = _fixture.Create<List<JigsawCustomerSearchApiResponseObject>>();

        _mockGetJigsawAuthTokenUseCase.Setup(x => x.Execute(redisId)).ReturnsAsync(jigsawToken);

        _mockJigsawGateway.Setup(x => x.GetCustomers(firstName, lastName, jigsawToken)).ReturnsAsync(stubbedEntity);

        var results = _classUnderTest.Execute(firstName, lastName, redisId).Result;

        results.SystemIds[^1].SystemName.Should().BeEquivalentTo(DataSource.Jigsaw);
        results.SystemIds[^1].Id.Should().BeEquivalentTo(searchText);
        results.SearchResponse.SearchResults[0].FirstName.Should().BeEquivalentTo(stubbedEntity[0].FirstName);
        results.SearchResponse.SearchResults[0].SurName.Should().BeEquivalentTo(stubbedEntity[0].LastName);
        results.SearchResponse.SearchResults[0].DateOfBirth.Should().Be(stubbedEntity[0].DoB);
        results.SearchResponse.SearchResults[0].DataSource.Should().BeEquivalentTo(DataSource.Jigsaw);
    }
}
