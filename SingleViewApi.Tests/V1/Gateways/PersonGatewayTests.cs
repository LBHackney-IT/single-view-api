using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using SingleViewApi.V1.Gateways;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class PersonGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private PersonGateway _classUnderTest;
        private MockHttpMessageHandler _mockHttp;

        [SetUp]
        public void Setup()
        {
            _mockHttp = new MockHttpMessageHandler();
            const string baseUrl = "https://person.api";
            var mockClient = _mockHttp.ToHttpClient();
            _classUnderTest = new PersonGateway(mockClient, baseUrl);
        }

        [Test]
        public void ARequestIsMade()
        {
            // Arrange
            const string id = "e749f036-3183-49cb-8504-59b76c1a8f88";
            const string userToken = "User token";

            _mockHttp.Expect($"https://person.api/persons/{id}")
                .WithHeaders("Authorization", userToken);
            // Act
            _ = _classUnderTest.GetPersonById(id, userToken);

            // Assert
            _mockHttp.VerifyNoOutstandingExpectation();
        }


        [Test]
        public async Task GetPersonByIdReturnsNullIfEntityDoesntExist()
        {
            // Arrange
            const string id = "e749f036-3183-49cb-8504-59b76c1a8f88";
            const string userToken = "User token";

            _mockHttp.Expect($"https://person.api/persons/{id}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.NotFound, x => new StringContent(id));

            var person = await _classUnderTest.GetPersonById(id, userToken);

            person.Should().BeNull();
        }

        [Test]
        public async Task GetPersonByIdReturnsNullIfUserIsUnAuthorised()
        {
            // Arrange
            const string id = "e749f036-3183-49cb-8504-59b76c1a8f88";
            const string userToken = "User token";

            _mockHttp.Expect($"https://person.api/persons/{id}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.Unauthorized, x => new StringContent(id));

            var person = await _classUnderTest.GetPersonById(id, userToken);

            person.Should().BeNull();
        }

        [Test]
        public async Task GetPersonByIdReturnsNullIfApiIsDown()
        {
            // Arrange
            const string id = "e749f036-3183-49cb-8504-59b76c1a8f88";
            const string userToken = "User token";

            _mockHttp.Expect($"https://person.api/persons/{id}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.ServiceUnavailable, x => new StringContent(id));

            var person = await _classUnderTest.GetPersonById(id, userToken);

            person.Should().BeNull();
        }

        [Test]
        public async Task DataFromApiIsReturned()
        {
            // Arrange
            const string id = "e749f036-3183-49cb-8504-59b76c1a8f88";
            const string userToken = "User token";

            _mockHttp.Expect($"https://person.api/persons/{id}")
                .WithHeaders("Authorization", userToken)
                .Respond("application/json",
                    "{\"id\": \"e8a8aa89-0b57-ffdb-cb9f-cb4c3e5ed463\", " +
                        "\"title\": \"Miss\", " +
                        "\"preferredTitle\": \"Miss\", " +
                        "\"preferredFirstName\": \"Luna\", " +
                        "\"preferredSurname\": \"Kitty\", " +
                        "\"preferredMiddleName\": \"\", " +
                        "\"firstName\": \"Luna\", " +
                        "\"middleName\": \"Purry\", " +
                        "\"surname\": \"Kitty\", " +
                        "\"placeOfBirth\": \"London\", " +
                        "\"dateOfBirth\": \"2021-01-07\"}");
            // Act
            var person = await _classUnderTest.GetPersonById(id, userToken);

            // Assert
            _mockHttp.VerifyNoOutstandingExpectation();
            Assert.AreEqual(person.FirstName, "Luna");
            Assert.AreEqual(person.Surname, "Kitty");
        }
    }
}
