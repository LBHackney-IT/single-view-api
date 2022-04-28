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
using SingleViewApi.V1.Boundary;

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
        public async Task GetsAllNotesById()
        {
            var stubbedEntities = new NoteResponseObjectList()
            {
                NoteResponseObjects = _fixture.CreateMany<NoteResponseObject>().ToList()
            };
            stubbedEntities.SortByCreatedAtDescending();

            var systemIdList = new SystemIdList() { SystemIds = _fixture.CreateMany<SystemId>().ToList() };
            var systemIds = systemIdList.ToJson();
            var targetId = systemIdList.SystemIds[^1].Id;
            var userToken = _fixture.Create<string>();
            var paginationToken = "";
            var pageSize = 0;

            _mockNotesGateway.Setup(x =>
                x.GetAllById(targetId, userToken, paginationToken, pageSize)).ReturnsAsync(stubbedEntities);

            var response = await _classUnderTest.Execute(systemIds, userToken, paginationToken, pageSize);

            response.SystemIds[^1].Should().BeEquivalentTo(systemIdList.SystemIds[^1]);

            var noteResponseObject = response.Notes.NoteResponseObjects[^1];
            var stubbedEntity = stubbedEntities.NoteResponseObjects[^1];

            noteResponseObject.Author.Should().BeEquivalentTo(stubbedEntity.Author);
            noteResponseObject.Categorisation.Should().BeEquivalentTo(stubbedEntity.Categorisation);
            noteResponseObject.Description.Should().BeEquivalentTo(stubbedEntity.Description);
            noteResponseObject.Highlight.Should().Be(stubbedEntity.Highlight);
            noteResponseObject.Id.Should().Be(stubbedEntity.Id);
            noteResponseObject.Title.Should().BeEquivalentTo(stubbedEntity.Title);
        }

        [Test]
        public async Task ReturnsSystemIdErrorIfGatewayErrors()
        {
            var systemIdList = new SystemIdList() { SystemIds = _fixture.CreateMany<SystemId>().ToList() };
            var systemIds = systemIdList.ToJson();
            var targetId = systemIdList.SystemIds[^1].Id;
            var userToken = _fixture.Create<string>();
            var paginationToken = "";
            var pageSize = 0;

            _mockNotesGateway.Setup(x =>
                x.GetAllById(targetId, userToken, paginationToken, pageSize)).ReturnsAsync((NoteResponseObjectList) null);

            var response = await _classUnderTest.Execute(systemIds, userToken, paginationToken, pageSize);

            response.SystemIds[^1].Error.Should().BeEquivalentTo(GetAllNotesByIdUseCase.NotFound);
        }
    }
}
