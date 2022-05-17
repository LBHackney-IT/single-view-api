using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase;

[TestFixture]
public class GetCombinedSearchResultsByNameUseCaseTests
{
    private Mock<IGetJigsawCustomersUseCase> _mockGetJigsawCustomersUseCase;
    private Mock<IGetSearchResultsByNameUseCase> _mockGetSearchResultsByNameUseCase;
    private GetCombinedSearchResultsByNameUseCase _classUnderTest;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _mockGetJigsawCustomersUseCase = new Mock<IGetJigsawCustomersUseCase>();
        _mockGetSearchResultsByNameUseCase = new Mock<IGetSearchResultsByNameUseCase>();
        _classUnderTest = new GetCombinedSearchResultsByNameUseCase(_mockGetSearchResultsByNameUseCase.Object, _mockGetJigsawCustomersUseCase.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void ReturnsAnErrorWithSystemIdsWhenNoResultsFound()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var redisId = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var page = _fixture.Create<int>();

        _mockGetSearchResultsByNameUseCase.Setup(x =>
                x.Execute(firstName, lastName, page, userToken))
            .ReturnsAsync(new SearchResponseObject()
            {
                SearchResponse = new SearchResponse()
                {
                    SearchResults = null,
                    Total = 0,
                },
                SystemIds = new List<SystemId>(new[] { new SystemId() { SystemName = "HousingSearchApi" } })

            });

        _mockGetJigsawCustomersUseCase.Setup(x =>
                x.Execute(firstName, lastName, redisId))
            .ReturnsAsync(new SearchResponseObject()
            {
                SearchResponse = new SearchResponse()
                {
                    SearchResults = null,
                    Total = 0,
                },
                SystemIds = new List<SystemId>(new[] { new SystemId() { SystemName = "Jigsaw" } })

            });

        var results = _classUnderTest.Execute(firstName, lastName, page, userToken, redisId).Result;

        results.SystemIds[0].SystemName.Should().BeEquivalentTo("HousingSearchApi");
        results.SystemIds[1].SystemName.Should().BeEquivalentTo("Jigsaw");


    }


}
