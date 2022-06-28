using System.Threading.Tasks;
using AutoFixture;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class CreateNoteUseCaseTests : LogCallAspectFixture
    {
        private Mock<INotesGateway> _mockNotesGateway;
        private CreateNoteUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockNotesGateway = new Mock<INotesGateway>();
            _classUnderTest = new CreateNoteUseCase(_mockNotesGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task CreatesNote()
        {
            var noteResponseObject = _fixture.Create<NotesApiResponseObject>();
            var createNoteRequest = _fixture.Create<CreateNoteRequest>();
            var userToken = _fixture.Create<string>();

            _mockNotesGateway.Setup(x =>
                x.CreateNote(createNoteRequest, userToken)).ReturnsAsync(noteResponseObject);

            var response = await _classUnderTest.Execute(createNoteRequest, userToken);

            Assert.AreEqual(noteResponseObject.Author, response.Author);
            Assert.AreEqual(noteResponseObject.Categorisation, response.Categorisation);
            Assert.AreEqual(noteResponseObject.Description, response.Description);
            Assert.AreEqual(noteResponseObject.Highlight, response.Highlight);
            Assert.AreEqual(noteResponseObject.Id, response.Id);
            Assert.AreEqual(noteResponseObject.Title, response.Title);
        }

        [Test]
        public async Task ReturnsNullIfGatewayErrors()
        {
            var createNoteRequest = _fixture.Create<CreateNoteRequest>();
            var userToken = _fixture.Create<string>();

            _mockNotesGateway.Setup(x =>
                x.CreateNote(createNoteRequest, userToken)).ReturnsAsync((NotesApiResponseObject) null);

            var response = await _classUnderTest.Execute(createNoteRequest, userToken);

            Assert.IsNull(response);
        }
    }
}
