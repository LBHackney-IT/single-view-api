using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Bogus.DataSets;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
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
        var stubbedJigsawDataSource = _fixture.Create<DataSource>();
        var stubbedHousingSearchDataSource = _fixture.Create<DataSource>();
        var expectedSystemIds = new List<SystemId>
        {
            new SystemId() { Id = searchTerm, SystemName = stubbedHousingSearchDataSource.Name, Error = SystemId.NotFoundMessage },
            new SystemId(){ Id = searchTerm, SystemName = stubbedJigsawDataSource.Name, Error = SystemId.NotFoundMessage }
        };

        _mockGetSearchResultsByNameUseCase.Setup(x =>
                x.Execute(firstName, lastName, userToken))
            .ReturnsAsync(new SearchResponseObject()
            {
                SearchResponse = new SearchResponse()
                {
                    SearchResults = null,
                    Total = 0,
                },
                SystemIds = new List<SystemId>(new[] { new SystemId() { SystemName = stubbedHousingSearchDataSource.Name, Id = searchTerm, Error = SystemId.NotFoundMessage } })

            });

        _mockGetJigsawCustomersUseCase.Setup(x =>
                x.Execute(firstName, lastName, redisId, userToken))
            .ReturnsAsync(new SearchResponseObject()
            {
                SearchResponse = new SearchResponse()
                {
                    SearchResults = null,
                    Total = 0,
                },
                SystemIds = new List<SystemId>(new[] { new SystemId() { SystemName = stubbedJigsawDataSource.Name, Id = searchTerm, Error = SystemId.NotFoundMessage } })

            });

        var results = _classUnderTest.Execute(firstName, lastName, userToken, redisId).Result;

        results.SystemIds.Should().BeEquivalentTo(expectedSystemIds);

    }

    [Test]
    public void ReturnsConcatenatedSearchResults()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var redisId = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var jigsawResults = _fixture.Create<SearchResponse>();
        var housingResults = _fixture.Create<SearchResponse>();
        var stubbedJigsawDataSource = _fixture.Create<DataSource>();
        var stubbedHousingSearchDataSource = _fixture.Create<DataSource>();
        var jigsawResponseObject = new SearchResponseObject()
        {
            SearchResponse = jigsawResults,
            SystemIds = new List<SystemId>()
            {
                new SystemId() { SystemName = stubbedJigsawDataSource.Name, Id = $"{firstName}+{lastName}" }
            }
        };
        var housingResponseObject = new SearchResponseObject()
        {
            SearchResponse = housingResults,
            SystemIds = new List<SystemId>()
            {
                new SystemId() { SystemName = stubbedHousingSearchDataSource.Name, Id = $"{firstName}+{lastName}" }
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
                new SystemId() { SystemName = stubbedHousingSearchDataSource.Name, Id = $"{firstName}+{lastName}" },
                new SystemId() { SystemName = stubbedHousingSearchDataSource.Name, Id = $"{firstName}+{lastName}" }
            }
        };

        _mockGetSearchResultsByNameUseCase.Setup(x =>
                x.Execute(firstName, lastName, userToken))
            .ReturnsAsync(housingResponseObject);

        _mockGetJigsawCustomersUseCase.Setup(x =>
                x.Execute(firstName, lastName, redisId, userToken))
            .ReturnsAsync(jigsawResponseObject);

        var results = _classUnderTest.Execute(firstName, lastName, userToken, redisId).Result;

        results.Should().BeEquivalentTo(expectedSearchResults);
    }

    [Test]
    public void ReturnsOnlyHousingSearchResultsWhenNoRedisIdIsGiven()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        string redisId = null;
        var userToken = _fixture.Create<string>();
        var housingResults = _fixture.Create<SearchResponse>();
        var stubbedHousingSearchDataSource = _fixture.Create<DataSource>();
        var housingResponseObject = new SearchResponseObject()
        {
            SearchResponse = housingResults,
            SystemIds = new List<SystemId>()
            {
                new SystemId() { SystemName = stubbedHousingSearchDataSource.Name, Id = $"{firstName}+{lastName}" }
            }
        };
        var expectedSearchResults = new SearchResponseObject()
        {
            SearchResponse = new SearchResponse()
            {
                SearchResults =
                    housingResults.SearchResults,
                Total = housingResults.Total
            },
            SystemIds = new List<SystemId>()
            {
                new SystemId() { SystemName = stubbedHousingSearchDataSource.Name, Id = $"{firstName}+{lastName}" }
            }
        };

        _mockGetSearchResultsByNameUseCase.Setup(x =>
                x.Execute(firstName, lastName, userToken))
            .ReturnsAsync(housingResponseObject);

        var results = _classUnderTest.Execute(firstName, lastName, userToken, redisId).Result;

        results.Should().BeEquivalentTo(expectedSearchResults);




    }

    [Test]
    public void SortResultsByRelevanceReturnsMatchingResultsFirst()
    {
        var testFirstName = "testFirstName";
        var testLastName = "testLastName";
        var stubbedHousingSearchDataSource = _fixture.Create<DataSource>();
        var testUnsortedData = new List<SearchResult> {
            new SearchResult() {
                Id = _fixture.Create<Guid>().ToString(),
                FirstName = "IrrelevantName",
                SurName = testLastName,
                DateOfBirth = DateTime.Now,
                DataSource = stubbedHousingSearchDataSource.Name
            },
            new SearchResult()
        {
            Id = _fixture.Create<Guid>().ToString(),
            FirstName = testFirstName,
            SurName = testLastName,
            DateOfBirth = DateTime.Now,
            DataSource = stubbedHousingSearchDataSource.Name

        },
            new SearchResult() {
                Id = _fixture.Create<Guid>().ToString(),
                FirstName = "AnotherIrrelevantName",
                SurName = "IrrelevantLastName",
                DateOfBirth = DateTime.Now,
                DataSource = stubbedHousingSearchDataSource.Name
            },
        };
        var expectedSortedData = new List<SearchResult>
        {
            new SearchResult()
            {
                Id = _fixture.Create<Guid>().ToString(),
                FirstName = testFirstName,
                SurName = testLastName,
                DateOfBirth = DateTime.Now,
                DataSource = stubbedHousingSearchDataSource.Name
            },
            new SearchResult(){
                Id = _fixture.Create<Guid>().ToString(),
                FirstName = "IrrelevantName",
                SurName = testLastName,
                DateOfBirth = DateTime.Now,
                DataSource = stubbedHousingSearchDataSource.Name
            },
            new SearchResult() {
                Id = _fixture.Create<Guid>().ToString(),
                FirstName = "AnotherIrrelevantName",
                SurName = "IrrelevantLastName",
                DateOfBirth = DateTime.Now,
                DataSource = stubbedHousingSearchDataSource.Name
            },
        };

        var results = _classUnderTest.SortResultsByRelevance(testFirstName, testLastName, testUnsortedData);

        results[0].FirstName.Should().BeEquivalentTo(expectedSortedData[0].FirstName);
        results[1].FirstName.Should().BeEquivalentTo(expectedSortedData[1].FirstName);
        results[2].FirstName.Should().BeEquivalentTo(expectedSortedData[2].FirstName);
    }
}
