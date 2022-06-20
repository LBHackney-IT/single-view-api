using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class SearchSingleViewUseCaseTests
    {
        private Mock<ICustomerGateway> _mockCustomerGateway;
        private Fixture _fixture;
        private SearchSingleViewUseCase _classUnderTest;

        [SetUp]

        public void SetUp()
        {
            _mockCustomerGateway = new Mock<ICustomerGateway>();
            _classUnderTest = new SearchSingleViewUseCase(_mockCustomerGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void ReturnsAnErrorWhenNoResultsFoundFromHousingSearchApi()
        {
            var firstName = _fixture.Create<string>();
            var lastName = _fixture.Create<string>();

            _mockCustomerGateway.Setup(x =>
                x.Search(firstName, lastName))
                    .Returns(new List<SavedCustomer>());


            var results = _classUnderTest.Execute(firstName, lastName);

            results.SystemIds[^1].SystemName.Should().BeEquivalentTo("Single View");
            results.SystemIds[^1].Id.Should().BeEquivalentTo($"{firstName} {lastName}");
            results.SystemIds[^1].Error.Should().BeEquivalentTo(SystemId.NotFoundMessage);
        }

        [Test]
        public void ReturnsSearchResultsFromHousingSearchApi()
        {
            var firstName = _fixture.Create<string>();
            var lastName = _fixture.Create<string>();
            var stubbedEntity = _fixture.Create<SavedCustomer>();

            _mockCustomerGateway.Setup(x =>
                x.Search(firstName, lastName)).Returns(new List<SavedCustomer>(){stubbedEntity});

            var results = _classUnderTest.Execute(firstName, lastName);

            results.SystemIds[^1].SystemName.Should().BeEquivalentTo("Single View");
            results.SystemIds[^1].Id.Should().BeEquivalentTo($"{firstName} {lastName}");

            results.SearchResponse.Total.Should().Be(1);
            results.SearchResponse.SearchResults[0].FirstName.Should()
                .BeEquivalentTo(stubbedEntity.FirstName);
            results.SearchResponse.SearchResults[0].SurName.Should()
                .BeEquivalentTo(stubbedEntity.LastName);
            results.SearchResponse.SearchResults[0].DateOfBirth.Should()
                .Be(stubbedEntity.DateOfBirth);
            results.SearchResponse.SearchResults[0].DataSource.Should().BeEquivalentTo("Single View");
        }
    }
}
