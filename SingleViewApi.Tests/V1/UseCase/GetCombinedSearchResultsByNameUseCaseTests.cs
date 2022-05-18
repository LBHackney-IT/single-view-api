using System.Collections.Generic;
using System.Linq;
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
        var searchTerm = $"{firstName}+{lastName}";
        var redisId = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var page = _fixture.Create<int>();
        var expectedSystemIds = new List<SystemId>
        {
            new SystemId() { Id = searchTerm, SystemName = "HousingSearchApi", Error = "no results found" },
            new SystemId(){ Id = searchTerm, SystemName = "Jigsaw", Error = "no results found" }
        };

        _mockGetSearchResultsByNameUseCase.Setup(x =>
                x.Execute(firstName, lastName, page, userToken))
            .ReturnsAsync(new SearchResponseObject()
            {
                SearchResponse = new SearchResponse()
                {
                    SearchResults = null,
                    Total = 0,
                },
                SystemIds = new List<SystemId>(new[] { new SystemId() { SystemName = "HousingSearchApi", Id = searchTerm, Error = "no results found" } })

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
                SystemIds = new List<SystemId>(new[] { new SystemId() { SystemName = "Jigsaw", Id = searchTerm, Error = "no results found" } })

            });

        var results = _classUnderTest.Execute(firstName, lastName, page, userToken, redisId).Result;

        results.SystemIds.Should().BeEquivalentTo(expectedSystemIds);

    }

    [Test]
    public void ReturnsConcatenatedSearchResults()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var redisId = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var page = _fixture.Create<int>();
        var jigsawResults = _fixture.Create<SearchResponse>();
        var housingResults = _fixture.Create<SearchResponse>();
        var jigsawResponseObject = new SearchResponseObject()
        {
            SearchResponse = jigsawResults,
            SystemIds = new List<SystemId>()
            {
                new SystemId() { SystemName = "Jigsaw", Id = $"{firstName}+{lastName}" }
            }
        };
        var housingResponseObject = new SearchResponseObject()
        {
            SearchResponse = housingResults,
            SystemIds = new List<SystemId>()
            {
                new SystemId() { SystemName = "HousingSearchApi", Id = $"{firstName}+{lastName}" }
            }
        };
        var expectedSearchResults = new SearchResponseObject()
        {
            SearchResponse = new SearchResponse()
            {
                SearchResults =
                    housingResults.SearchResults
                        .Concat(jigsawResults.SearchResults).ToList(),
                Total = housingResults.Total + jigsawResults.Total
            },
            SystemIds = new List<SystemId>()
            {
                new SystemId() { SystemName = "Jigsaw", Id = $"{firstName}+{lastName}" },
                new SystemId() { SystemName = "HousingSearchApi", Id = $"{firstName}+{lastName}" }
            }
        };

        _mockGetSearchResultsByNameUseCase.Setup(x =>
                x.Execute(firstName, lastName, page, userToken))
            .ReturnsAsync(housingResponseObject);

        _mockGetJigsawCustomersUseCase.Setup(x =>
                x.Execute(firstName, lastName, redisId))
            .ReturnsAsync(jigsawResponseObject);

        var results = _classUnderTest.Execute(firstName, lastName, page, userToken, redisId).Result;

        results.Should().BeEquivalentTo(expectedSearchResults);




    }


}
