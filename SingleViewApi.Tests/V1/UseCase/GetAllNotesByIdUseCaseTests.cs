using System.Collections.Generic;
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
            var notesResponseFixture = new NotesResponse()
            {
                Notes = _fixture.CreateMany<NoteResponseObject>().ToList(),
                SystemIds = _fixture.CreateMany<SystemId>().ToList()
            };
            notesResponseFixture.SortByCreatedAtDescending();

            var systemIdList = new SystemIdList() { SystemIds = _fixture.CreateMany<SystemId>().ToList() };
            var systemIds = systemIdList.ToJson();
            var targetId = systemIdList.SystemIds[^1].Id;
            var userToken = _fixture.Create<string>();
            var paginationToken = "";
            var pageSize = 0;

            _mockNotesGateway.Setup(x =>
                x.GetAllById(targetId, userToken, paginationToken, pageSize)).ReturnsAsync(notesResponseFixture.Notes);

            var response = await _classUnderTest.Execute(systemIds, userToken, paginationToken, pageSize);

            response.SystemIds[^1].Should().BeEquivalentTo(systemIdList.SystemIds[^1]);

            var note = response.Notes[^1];
            var noteFixture = notesResponseFixture.Notes[^1];

            note.Author.Should().BeEquivalentTo(noteFixture.Author);
            note.Categorisation.Should().BeEquivalentTo(noteFixture.Categorisation);
            note.Description.Should().BeEquivalentTo(noteFixture.Description);
            note.Highlight.Should().Be(noteFixture.Highlight);
            note.Id.Should().Be(noteFixture.Id);
            note.Title.Should().BeEquivalentTo(noteFixture.Title);
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
                x.GetAllById(targetId, userToken, paginationToken, pageSize)).ReturnsAsync((List<NoteResponseObject>) null);

            var response = await _classUnderTest.Execute(systemIds, userToken, paginationToken, pageSize);

            response.SystemIds[^1].Error.Should().BeEquivalentTo(GetAllNotesByIdUseCase.NotFound);
        }
    }
}
