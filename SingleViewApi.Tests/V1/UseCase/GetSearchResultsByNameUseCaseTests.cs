using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetSearchResultsByNameUseCaseTests
    {
        private Mock<IHousingSearchGateway> _mockHousingSearchGateway;
        private GetSearchResultsByNameUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]

        public void SetUp()
        {
            _mockHousingSearchGateway = new Mock<IHousingSearchGateway>();
            _classUnderTest = new GetSearchResultsByNameUseCase(_mockHousingSearchGateway.Object);
            _fixture = new Fixture();
        }

        [Test]

        public async Task ReturnsAnErrorWhenNoResultsFoundFromHousingSearchApi()
        {
            var firstName = _fixture.Create<string>();
            var lastName = _fixture.Create<string>();
            var searchText = $"{firstName}+{lastName}";
            var userToken = _fixture.Create<string>();
            var page = _fixture.Create<int>();
            _mockHousingSearchGateway.Setup(x =>
                x.GetSearchResultsBySearchText(searchText, page, userToken))
                    .ReturnsAsync((HousingSearchApiResponse) null);

            var results = await _classUnderTest.Execute(firstName,lastName, page, userToken);

            results.SystemIds[^1].SystemName.Should().BeEquivalentTo("HousingSearchApi");
            results.SystemIds[^1].Id.Should().BeEquivalentTo(searchText);
            results.SystemIds[^1].Error.Should().BeEquivalentTo("No results found");
        }

        [Test]

        public async Task ReturnsSearchResultsFromHousingSearchApi()
        {
            var firstName = _fixture.Create<string>();
            var lastName = _fixture.Create<string>();
            var searchText = $"{firstName}+{lastName}";
            var userToken = _fixture.Create<string>();
            var page = _fixture.Create<int>();
            var stubbedEntity = _fixture.Create<HousingSearchApiResponse>();

            _mockHousingSearchGateway.Setup(x => x.GetSearchResultsBySearchText(searchText, page, userToken))
                .ReturnsAsync(stubbedEntity);

            var results = await _classUnderTest.Execute(firstName, lastName, page, userToken);

            results.SystemIds[^1].SystemName.Should().BeEquivalentTo("HousingSearchApi");
            results.SystemIds[^1].Id.Should().BeEquivalentTo(searchText);

            results.SearchResponse.Total.Should().Be(stubbedEntity.Total);
            results.SearchResponse.SearchResults[0].FirstName.Should()
                .BeEquivalentTo(stubbedEntity.Results.Persons[0].FirstName);
            results.SearchResponse.SearchResults[0].SurName.Should()
                .BeEquivalentTo(stubbedEntity.Results.Persons[0].Surname);
            results.SearchResponse.SearchResults[0].PersonTypes[0].Should()
                .BeEquivalentTo(stubbedEntity.Results.Persons[0].PersonTypes.ToList()[0]);
            results.SearchResponse.SearchResults[0].DateOfBirth.Should()
                .Be(stubbedEntity.Results.Persons[0].DateOfBirth);
            results.SearchResponse.SearchResults[0].KnownAddresses[0].FullAddress.Should()
                .BeEquivalentTo(stubbedEntity.Results.Persons[0].Tenures.ToList()[0].AssetFullAddress);
        }
    }
}
