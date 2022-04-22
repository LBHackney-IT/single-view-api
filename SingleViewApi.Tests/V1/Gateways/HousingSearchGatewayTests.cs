using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class HousingSearchGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private HousingSearchGateway _classUnderTest;
        private MockHttpMessageHandler _mockHttp;

        [SetUp]

        public void Setup()
        {
            _mockHttp = new MockHttpMessageHandler();
            const string baseUrl = "https://housingsearch.api";
            var mockClient = _mockHttp.ToHttpClient();
            _classUnderTest = new HousingSearchGateway(mockClient, baseUrl);

        }

        [Test]

        public void ARequestIsMade()
        {
            // Arrange
            const string searchText = "Search-Text";
            const string userToken = "User token";

            _mockHttp.Expect($"https://housingsearch.api/search/persons?searchText={searchText}")
                .WithHeaders("Authorization", userToken);
            // Act
            _ = _classUnderTest.GetSearchResultsBySearchText(searchText, userToken);

            // Assert
            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]

        public async Task SearchBySearchTextReturnsNullIfThereAreNoResults()
        {
            //Arrange

            const string searchText = "Test";
            const string userToken = "User token";

            _mockHttp.Expect($"https://housingsearch.api/search/persons?searchText={searchText}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.NotFound, x => new StringContent(searchText));

            //Act
            var searchResults = await _classUnderTest.GetSearchResultsBySearchText(searchText, userToken);

            //Assert

            searchResults.Should().BeNull();
        }

        [Test]
        public async Task SearchBySearchTextReturnsNullIfUserIsUnAuthorised()
        {
            // Arrange
            const string searchText = "Test";
            const string userToken = "User token";

            _mockHttp.Expect($"https://housingsearch.api/search/persons?searchText={searchText}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.Unauthorized, x => new StringContent(searchText));

            var searchResults = await _classUnderTest.GetSearchResultsBySearchText(searchText, userToken);

            searchResults.Should().BeNull();
        }

    }
}
