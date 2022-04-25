using System;
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
    public class GetSearchResultsBySearchTextUseCaseTests
    {
        private Mock<IHousingSearchGateway> _mockHousingSearchGateway;
        private GetSearchResultsBySearchTextUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]

        public void SetUp()
        {
            _mockHousingSearchGateway = new Mock<IHousingSearchGateway>();
            _classUnderTest = new GetSearchResultsBySearchTextUseCase(_mockHousingSearchGateway.Object);
            _fixture = new Fixture();
        }

        [Test]

        public async Task ReturnsAnErrorWhenNoResultsFoundFromHousingSearchApi()
        {
            var searchText = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();
            _mockHousingSearchGateway.Setup(x =>
                x.GetSearchResultsBySearchText(searchText, userToken))
                    .ReturnsAsync((HousingSearchApiResponse)null);

            var results = await _classUnderTest.Execute(searchText, userToken);

            results.SystemIds[^1].SystemName.Should().BeEquivalentTo("HousingSearchApi");
            results.SystemIds[^1].Id.Should().BeEquivalentTo(searchText);
            results.SystemIds[^1].Error.Should().BeEquivalentTo("No results found");
        }

        [Test]

        public async Task ReturnsSearchResultsFromHousingSearchApi()
        {
            var searchText = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();
            var stubbedEntity = _fixture.Create<HousingSearchApiResponse>();

            _mockHousingSearchGateway.Setup(x => x.GetSearchResultsBySearchText(searchText, userToken))
                .ReturnsAsync(stubbedEntity);

            var results = await _classUnderTest.Execute(searchText, userToken);

            results.SystemIds[^1].SystemName.Should().BeEquivalentTo("HousingSearchApi");
            results.SystemIds[^1].Id.Should().BeEquivalentTo(searchText);




            results.SearchResponse.Results.Total.Should().Be(stubbedEntity.Total);
            results.SearchResponse.Results.Results.Persons[0].Firstname.Should()
                .BeEquivalentTo(stubbedEntity.Results.Persons[0].Firstname);
            results.SearchResponse.Results.Results.Persons[0].Surname.Should()
                .BeEquivalentTo(stubbedEntity.Results.Persons[0].Surname);
            results.SearchResponse.Results.Results.Persons[0].PersonTypes[0].Should()
                .BeEquivalentTo(stubbedEntity.Results.Persons[0].PersonTypes[0]);
            results.SearchResponse.Results.Results.Persons[0].DateOfBirth.Should()
                .BeEquivalentTo(stubbedEntity.Results.Persons[0].DateOfBirth);
          }
    }
}
