using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SingleViewApi.V1.Gateways;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class NotesGatewayTests
    {
        private NotesGateway _classUnderTest;
        private MockHttpMessageHandler _mockHttp;

        [SetUp]
        public void Setup()
        {
            _mockHttp = new MockHttpMessageHandler();
            const string baseUrl = "https://api.notes";
            var mockClient = _mockHttp.ToHttpClient();
            _classUnderTest = new NotesGateway(mockClient, baseUrl);
        }

        [Test]
        public void ARequestIsMade()
        {
            const string id = "id";
            const string userToken = "user-token";

            _mockHttp.Expect($"https://api.notes/notes?targetId={id}")
                .WithHeaders("Authorization", userToken);

            _ = _classUnderTest.GetAllById(id, userToken);

            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task GetAllByIdReturnsNullIfNotesDoNotExist()
        {
            const string id = "id";
            const string userToken = "user-token";

            _mockHttp.Expect($"https://api.notes/notes?targetId={id}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.NotFound, x => new StringContent(id));

            var notes = await _classUnderTest.GetAllById(id, userToken);

            notes.Should().BeNull();
        }

        [Test]
        public async Task GetAllByIdReturnsNullIfUserIsUnauthorised()
        {
            const string id = "id";
            const string userToken = "user-token";

            _mockHttp.Expect($"https://api.notes/notes?targetId={id}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.Unauthorized, x => new StringContent(id));

            var notes = await _classUnderTest.GetAllById(id, userToken);

            notes.Should().BeNull();
        }

        [Test]
        public async Task GetAllByIdReturnsNullIfServiceUnavailable()
        {
            const string id = "id";
            const string userToken = "user-token";

            _mockHttp.Expect($"https://api.notes/notes?targetId={id}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.ServiceUnavailable, x => new StringContent(id));

            var notes = await _classUnderTest.GetAllById(id, userToken);

            notes.Should().BeNull();
        }

        [Test]
        public async Task DataIsReturned()
        {
            const string id = "id";
            const string userToken = "user-token";

            _mockHttp.Expect($"https://api.notes/notes?targetId={id}")
                .WithHeaders("Authorization", userToken)
                .Respond("application/json",
                    "{\"results\": [" +
                    "{\"id\": \"15c89eb9-ffc6-4ebe-9f1e-fcbb717d0ce4\", " +
                    "\"title\": \"Test note 1\", " +
                    "\"description\": \"Pluribus ad comitatum imperatoris vinctum perduceret.\", " +
                    "\"targetType\": \"person\", " +
                    "\"targetId\": \"e8a8aa89-0b57-ffdb-cb9f-cb4c3e5ed463\", " +
                    "\"createdAt\": \"2022-04-14T15:08:13.765Z\", " +
                    "\"categorisation\": {\"category\": \"repairs\", " +
                    "\"subCategory\": null, \"description\": null}, " +
                    "\"author\": {\"fullName\": \"Test user\", " +
                    "\"email\": \"test.user@hackney.gov.uk\"}, " +
                    "\"highlight\": false}, " +
                    "{\"id\": \"c6407343-3607-422f-9a44-e4f44e55ee18\", " +
                    "\"title\": \"Test note 2\", " +
                    "\"description\": \"quo percitus ille exitio urgente abrupto ferro eundem adoritur paulum.\", " +
                    "\"targetType\": \"person\", " +
                    "\"targetId\": \"e8a8aa89-0b57-ffdb-cb9f-cb4c3e5ed463\", " +
                    "\"createdAt\": \"2022-04-14T15:08:13.765Z\", " +
                    "\"categorisation\": {\"category\": \"repairs\", " +
                    "\"subCategory\": null, \"description\": null}, " +
                    "\"author\": {\"fullName\": \"Test user\", " +
                    "\"email\": \"test.user@hackney.gov.uk\"}, " +
                    "\"highlight\": false}" +
                    "]}");

            var results = await _classUnderTest.GetAllById(id, userToken);

            _mockHttp.VerifyNoOutstandingExpectation();
            Assert.AreEqual(results.NoteResponseObjects[0].Title, "Test note 1");
            Assert.AreEqual(results.NoteResponseObjects[1].Title, "Test note 2");
        }
    }
}
