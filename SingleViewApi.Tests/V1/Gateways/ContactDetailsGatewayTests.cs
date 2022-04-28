using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Hackney.Shared.ContactDetail.Domain;
using SingleViewApi.V1.Gateways;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class ContactDetailsGatewayTest
    {
        private readonly Fixture _fixture = new Fixture();
        private ContactDetailsGateway _classUnderTest;
        private MockHttpMessageHandler _mockHttp;


        [SetUp]
        public void Setup()
        {
            _mockHttp = new MockHttpMessageHandler();
            const string baseUrl = "https://contact-details.api";
            var mockClient = _mockHttp.ToHttpClient();
            _classUnderTest = new ContactDetailsGateway(mockClient, baseUrl);

        }

        [Test]
        public void ARequestIsMade()
        {
            // Arrange
            const string id = "123-Some-ID";
            const string userToken = "User token";

            _mockHttp.Expect($"https://contact-details.api/contactDetails?targetId={id}&includeHistoric=true")
                .WithHeaders("Authorization", userToken);
            // Act
            _ = _classUnderTest.GetContactDetailsById(id, userToken);

            // Assert
            _mockHttp.VerifyNoOutstandingExpectation();
        }


        [Test]
        public async Task GetContactDetailsByIdReturnsNullIfEntityDoesntExist()
        {
            // Arrange
            const string id = "123-Some-ID";
            const string userToken = "User token";

            _mockHttp.Expect($"https://contact-details.api/contactDetails?targetId={id}&includeHistoric=true")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.NotFound, x => new StringContent(id));

            var contactDetails = await _classUnderTest.GetContactDetailsById(id, userToken);

            contactDetails.Should().BeNull();
        }

        [Test]
        public async Task GetContactDetailsByIdReturnsNullIfUserIsUnAuthorised()
        {
            // Arrange
            const string id = "123-Some-ID";
            const string userToken = "User token";

            _mockHttp.Expect($"https://contact-details.api/contactDetails?targetId={id}&includeHistoric=true")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.Unauthorized, x => new StringContent(id));

            var contactDetails = await _classUnderTest.GetContactDetailsById(id, userToken);

            contactDetails.Should().BeNull();
        }

        [Test]
        public async Task GetContactDetailsByIdReturnsNullIfApiIsDown()
        {
            // Arrange
            const string id = "123-Some-ID";
            const string userToken = "User token";

            _mockHttp.Expect($"https://contact-details.api/contactDetails?targetId={id}&includeHistoric=true")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.ServiceUnavailable, x => new StringContent(id));

            var contactDetails = await _classUnderTest.GetContactDetailsById(id, userToken);

            contactDetails.Should().BeNull();
        }

        [Test]
        public async Task DataFromApiIsReturned()
        {
            // Arrange
            const string id = "123-Some-ID";
            const string userToken = "User token";
            var stubbedContactDetails = _fixture.Create<ContactDetails>();


            _mockHttp.Expect($"https://contact-details.api/contactDetails?targetId={id}&includeHistoric=true")
                .WithHeaders("Authorization", userToken)
                .Respond("application/json",
                    JsonSerializer.Serialize<ContactDetails>(stubbedContactDetails));

            // Act
            var contactDetails = await _classUnderTest.GetContactDetailsById(id, userToken);

            // Assert
            _mockHttp.VerifyNoOutstandingExpectation();
            Assert.AreEqual(stubbedContactDetails.ContactInformation.Description, contactDetails.ContactInformation.Description);
            Assert.AreEqual(stubbedContactDetails.CreatedBy.FullName, contactDetails.CreatedBy.FullName);
            Assert.AreEqual(stubbedContactDetails.IsActive, contactDetails.IsActive);
            Assert.AreEqual(stubbedContactDetails.RecordValidUntil, contactDetails.RecordValidUntil);
            Assert.AreEqual(stubbedContactDetails.SourceServiceArea.Area, contactDetails.SourceServiceArea.Area);
        }
    }
}
