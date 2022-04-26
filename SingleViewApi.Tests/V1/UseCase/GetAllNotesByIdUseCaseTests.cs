using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetAllNotesByIdUseCaseTests : LogCallAspectFixture
    {
        private Mock<INotesGateway> _mockNotesGateway;
        private GetAllNotesByIdUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockNotesGateway = new Mock<INotesGateway>();
            _classUnderTest = new GetAllNotesByIdUseCase(_mockNotesGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetsAllNotesByIdFromTheGateway()
        {
            var stubbedEntities = new NoteResponseObjectList()
            {
                NoteResponseObjects = _fixture.CreateMany<NoteResponseObject>().ToList()
            };

            var id = _fixture.Create<string>();
            var userToken = _fixture.Create<string>();
            var paginationToken = "";
            var pageSize = 0;

            _mockNotesGateway.Setup(x =>
                x.GetAllById(id, userToken, paginationToken, pageSize)).ReturnsAsync(stubbedEntities);

            var response = await _classUnderTest.Execute(id, userToken, paginationToken, pageSize);

            response.Notes.NoteResponseObjects[^1].Description.Should()
                .BeEquivalentTo(stubbedEntities.NoteResponseObjects[^1].Description);
        }
    }
}
