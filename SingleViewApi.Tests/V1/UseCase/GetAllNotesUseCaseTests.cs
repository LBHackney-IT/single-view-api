using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetAllNotesUseCaseTests : LogCallAspectFixture
    {
        private Mock<IGetNotesUseCase> _mockGetNotesUseCase;
        private Mock<IGetJigsawNotesUseCase> _mockGetJigsawNotesUseCase;
        private GetAllNotesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGetNotesUseCase = new Mock<IGetNotesUseCase>();
            _mockGetJigsawNotesUseCase = new Mock<IGetJigsawNotesUseCase>();
            _classUnderTest = new GetAllNotesUseCase(_mockGetNotesUseCase.Object, _mockGetJigsawNotesUseCase.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetsAllNotes()
        {
            var systemIdListFixture = new SystemIdList() { SystemIds = _fixture.CreateMany<SystemId>().ToList() };
            var systemIds = systemIdListFixture.ToJson();
            var id = systemIdListFixture.SystemIds[^1].Id;
            var userToken = _fixture.Create<string>();
            var redisKey = _fixture.Create<string>();
            var paginationToken = "";
            var pageSize = 0;

            var notesFixture = new List<NoteResponseObject>();
            var notesApiNoteResponseObjectListFixture = _fixture.CreateMany<NoteResponseObject>().ToList();
            var jigsawNoteResponseObjectListFixture = _fixture.CreateMany<NoteResponseObject>().ToList();
            notesFixture.AddRange(notesApiNoteResponseObjectListFixture);
            notesFixture.AddRange(jigsawNoteResponseObjectListFixture);
            var notesResponseFixture = new NotesResponse()
            {
                Notes = notesFixture,
                SystemIds = systemIdListFixture.SystemIds
            };
            notesResponseFixture.SortByCreatedAtDescending();

            _mockGetNotesUseCase.Setup(x =>
                x.Execute(id, userToken, paginationToken, pageSize)).ReturnsAsync(notesApiNoteResponseObjectListFixture);

            _mockGetJigsawNotesUseCase.Setup(x =>
                x.Execute(id, redisKey)).ReturnsAsync(jigsawNoteResponseObjectListFixture);

            var response = await _classUnderTest.Execute(systemIds, userToken, redisKey, paginationToken, pageSize);
            response.SystemIds[^1].Should().BeEquivalentTo(systemIdListFixture.SystemIds[^1]);

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
            var systemIdListFixture = new SystemIdList() { SystemIds = _fixture.CreateMany<SystemId>().ToList() };
            var systemIds = systemIdListFixture.ToJson();
            var id = systemIdListFixture.SystemIds[^1].Id;
            var userToken = _fixture.Create<string>();
            var redisKey = _fixture.Create<string>();
            var paginationToken = "";
            var pageSize = 0;

            _mockGetNotesUseCase.Setup(x =>
                x.Execute(id, userToken, paginationToken, pageSize)).ReturnsAsync((List<NoteResponseObject>) null);

            _mockGetJigsawNotesUseCase.Setup(x =>
                x.Execute(id, redisKey)).ReturnsAsync((List<NoteResponseObject>) null);

            var response = await _classUnderTest.Execute(systemIds, userToken, redisKey, paginationToken, pageSize);
            response.SystemIds[^1].Id.Should().BeEquivalentTo(systemIdListFixture.SystemIds[^1].Id);
            response.SystemIds[^1].SystemName.Should().BeEquivalentTo(systemIdListFixture.SystemIds[^1].SystemName);
            response.SystemIds[^1].Error.Should().BeEquivalentTo(GetAllNotesUseCase.NotFound);
        }
    }
}
