using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Newtonsoft.Json;
using SingleViewApi.V1.Gateways;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class NotesGatewayTests
    {
        private NotesGateway _classUnderTest;
        private MockHttpMessageHandler _mockHttp;
        private string _baseUrl;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _baseUrl = "https://api.notes";

            _mockHttp = new MockHttpMessageHandler();
            var mockClient = _mockHttp.ToHttpClient();

            _classUnderTest = new NotesGateway(mockClient, _baseUrl);
        }

        [Test]
        public void ARequestIsMade()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();

            _mockHttp.Expect($"{_baseUrl}/notes?targetId={id}")
                .WithHeaders("Authorization", userToken);

            _ = _classUnderTest.GetAllById(id, userToken);

            _mockHttp.VerifyNoOutstandingExpectation();
        }

        [Test]
        public async Task GetAllByIdReturnsNullIfNotesDoNotExist()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();

            _mockHttp.Expect($"{_baseUrl}/notes?targetId={id}")
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

            _mockHttp.Expect($"{_baseUrl}/notes?targetId={id}")
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

            _mockHttp.Expect($"{_baseUrl}/notes?targetId={id}")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.ServiceUnavailable, x => new StringContent(id));

            var notes = await _classUnderTest.GetAllById(id, userToken);

            Assert.Null(notes);
        }

        [Test]
        public async Task GetAllByIdReturnsData()
        {
            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();
            var notesResultsResponseObject = new NotesResultsResponseObject() { Results = _fixture.CreateMany<NotesApiResponseObject>().ToList() };

            _mockHttp.Expect($"{_baseUrl}/notes?targetId={id}")
                .WithHeaders("Authorization", userToken)
                .Respond("application/json", notesResultsResponseObject.ToJson());

            var results = await _classUnderTest.GetAllById(id, userToken);

            _mockHttp.VerifyNoOutstandingExpectation();

            var noteResponseObject = notesResultsResponseObject.Results[^1];
            var noteResponseObjectResult = results[^1];

            Assert.AreEqual(noteResponseObject.Id, noteResponseObjectResult.Id);
            Assert.AreEqual(noteResponseObject.Title, noteResponseObjectResult.Title);
            Assert.AreEqual(noteResponseObject.Description, noteResponseObjectResult.Description);
        }

        [Test]
        public async Task CreateNoteReturnsNullIfBadRequest()
        {
            var userToken = _fixture.Create<string>();
            var noteResponseObject = _fixture.Create<NoteResponseObject>();
            var noteResponseJson = noteResponseObject.ToJson();
            var createNoteRequest = JsonConvert.DeserializeObject<CreateNoteRequest>(noteResponseJson);

            _mockHttp.Expect($"{_baseUrl}/notes")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.BadRequest, x =>
                    new StringContent(noteResponseObject.TargetId.ToString()));

            var note = await _classUnderTest.CreateNote(createNoteRequest, userToken);

            Assert.Null(note);
        }

        [Test]
        public async Task CreateNoteReturnsNullIfInternalServerError()
        {
            var userToken = _fixture.Create<string>();
            var noteResponseObject = _fixture.Create<NoteResponseObject>();
            var noteResponseJson = noteResponseObject.ToJson();
            var createNoteRequest = JsonConvert.DeserializeObject<CreateNoteRequest>(noteResponseJson);

            _mockHttp.Expect($"{_baseUrl}/notes")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.InternalServerError, x =>
                    new StringContent(noteResponseObject.TargetId.ToString()));

            var note = await _classUnderTest.CreateNote(createNoteRequest, userToken);

            Assert.Null(note);
        }

        [Test]
        public async Task CreateNoteReturnsNullIfUserIsUnauthorised()
        {
            var userToken = _fixture.Create<string>();
            var noteResponseObject = _fixture.Create<NoteResponseObject>();
            var noteResponseJson = noteResponseObject.ToJson();
            var createNoteRequest = JsonConvert.DeserializeObject<CreateNoteRequest>(noteResponseJson);

            _mockHttp.Expect($"{_baseUrl}/notes")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.Unauthorized, x =>
                    new StringContent(noteResponseObject.TargetId.ToString()));

            var note = await _classUnderTest.CreateNote(createNoteRequest, userToken);

            Assert.Null(note);
        }

        [Test]
        public async Task CreateNoteReturnsNullIfServiceUnavailable()
        {
            var userToken = _fixture.Create<string>();
            var noteResponseObject = _fixture.Create<NoteResponseObject>();
            var noteResponseJson = noteResponseObject.ToJson();
            var createNoteRequest = JsonConvert.DeserializeObject<CreateNoteRequest>(noteResponseJson);

            _mockHttp.Expect($"{_baseUrl}/notes")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.ServiceUnavailable, x =>
                    new StringContent(noteResponseObject.TargetId.ToString()));

            var note = await _classUnderTest.CreateNote(createNoteRequest, userToken);

            Assert.Null(note);
        }

        [Test]
        public async Task CreateNoteReturnsData()
        {
            var userToken = _fixture.Create<string>();
            var noteResponseObject = _fixture.Create<NoteResponseObject>();
            var noteResponseJson = noteResponseObject.ToJson();
            var createNoteRequest = JsonConvert.DeserializeObject<CreateNoteRequest>(noteResponseJson);

            _mockHttp.Expect($"{_baseUrl}/notes")
                .WithHeaders("Authorization", userToken)
                .Respond(HttpStatusCode.Created, "application/json", noteResponseJson);

            var note = await _classUnderTest.CreateNote(createNoteRequest, userToken);

            _mockHttp.VerifyNoOutstandingExpectation();

            Assert.AreEqual(noteResponseObject.Id, note.Id);
            Assert.AreEqual(noteResponseObject.Title, note.Title);
            Assert.AreEqual(noteResponseObject.Description, note.Description);
        }
    }
}
