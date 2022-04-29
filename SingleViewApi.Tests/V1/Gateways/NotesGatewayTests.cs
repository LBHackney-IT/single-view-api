using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using SingleViewApi.V1.Gateways;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class NotesGatewayTests
    {
        private NotesGateway _classUnderTest;
        private MockHttpMessageHandler _mockHttp;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _mockHttp = new MockHttpMessageHandler();
            const string baseUrl = "https://api.notes";
            var mockClient = _mockHttp.ToHttpClient();
            _classUnderTest = new NotesGateway(mockClient, baseUrl);
            _fixture = new Fixture();
        }

        [Test]
        public void ARequestIsMade()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();

            _mockHttp.Expect($"https://api.notes/notes?targetId={id}")
                .WithHeaders("Authorization", userToken);

            _ = _classUnderTest.GetAllById(id, userToken);

            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task GetAllByIdReturnsNullIfNotesDoNotExist()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();

            _mockHttp.Expect($"https://api.notes/notes?targetId={id}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.NotFound, x => new StringContent(id));

            var notes = await _classUnderTest.GetAllById(id, userToken);

            Assert.Null(notes);
        }

        [Test]
        public async Task GetAllByIdReturnsNullIfUserIsUnauthorised()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();

            _mockHttp.Expect($"https://api.notes/notes?targetId={id}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.Unauthorized, x => new StringContent(id));

            var notes = await _classUnderTest.GetAllById(id, userToken);

            Assert.Null(notes);
        }

        [Test]
        public async Task GetAllByIdReturnsNullIfServiceUnavailable()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();

            _mockHttp.Expect($"https://api.notes/notes?targetId={id}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.ServiceUnavailable, x => new StringContent(id));

            var notes = await _classUnderTest.GetAllById(id, userToken);

            Assert.Null(notes);
        }

        [Test]
        public async Task DataIsReturned()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();
            var notesResultsResponseObject = new NotesResultsResponseObject() { Results = _fixture.CreateMany<NoteResponseObject>().ToList() };

            _mockHttp.Expect($"https://api.notes/notes?targetId={id}")
                .WithHeaders("Authorization", userToken)
                .Respond("application/json", notesResultsResponseObject.ToJson());

            var results = await _classUnderTest.GetAllById(id, userToken);

            _mockHttp.VerifyNoOutstandingExpectation();

            var noteResponseObject = notesResultsResponseObject.Results[^1];
            var noteResponseObjectResult = results.NoteResponseObjects[^1];

            Assert.AreEqual(noteResponseObject.Id, noteResponseObjectResult.Id);
            Assert.AreEqual(noteResponseObject.Title, noteResponseObjectResult.Title);
            Assert.AreEqual(noteResponseObject.Description, noteResponseObjectResult.Description);
        }
    }
}
