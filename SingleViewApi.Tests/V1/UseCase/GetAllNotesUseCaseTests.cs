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
using SingleViewApi.V1.Domain;
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
            var personApiSystemIdFixture = _fixture.Build<SystemId>()
                .With(o => o.SystemName, DataSource.PersonApi).Create();
            var personApiSystemId = personApiSystemIdFixture.Id;
            var jigsawSystemIdFixture = _fixture.Build<SystemId>()
                .With(o => o.SystemName, DataSource.Jigsaw).Create();
            var jigsawSystemId = jigsawSystemIdFixture.Id;
            var systemIdListFixture = new SystemIdList()
            {
                SystemIds = new List<SystemId>() { personApiSystemIdFixture, jigsawSystemIdFixture }
            };
            var systemIds = systemIdListFixture.ToJson();

            var userToken = _fixture.Create<string>();
            var redisKey = _fixture.Create<string>();
            var paginationToken = "";
            var pageSize = 0;

            var notesFixture = new List<NoteResponseObject>();
            var notesApiNoteResponseObjectListFixture = _fixture.Build<NoteResponseObject>()
                .With(o => o.DataSource, DataSource.NotesApi).CreateMany().ToList();
            var jigsawNoteResponseObjectListFixture = _fixture.Build<NoteResponseObject>()
                .With(o => o.DataSource, DataSource.Jigsaw).CreateMany().ToList();
            notesFixture.AddRange(notesApiNoteResponseObjectListFixture);
            notesFixture.AddRange(jigsawNoteResponseObjectListFixture);
            var notesResponseFixture = new NotesResponse()
            {
                Notes = notesFixture,
                SystemIds = systemIdListFixture.SystemIds
            };
            notesResponseFixture.Sort();

            _mockGetNotesUseCase.Setup(x =>
                x.Execute(personApiSystemId, userToken, paginationToken, pageSize)).ReturnsAsync(notesApiNoteResponseObjectListFixture);

            _mockGetJigsawNotesUseCase.Setup(x =>
                x.Execute(jigsawSystemId, redisKey)).ReturnsAsync(jigsawNoteResponseObjectListFixture);

            var response = await _classUnderTest.Execute(systemIds, userToken, redisKey, paginationToken, pageSize);
            Assert.AreEqual(systemIdListFixture.SystemIds[^1].Id, response.SystemIds[^1].Id);
            Assert.AreEqual(notesResponseFixture.Notes.Count, response.Notes.Count);
            Assert.AreEqual(notesResponseFixture.Notes[^1].Id, response.Notes[^1].Id);
        }

        [Test]
        public async Task IgnoresJigsawNotesIfRedisKeyIsNull()
        {
            var jigsawSystemIdFixture = _fixture.Build<SystemId>()
                .With(o => o.SystemName, DataSource.Jigsaw).Create();
            var customerIdFixture = jigsawSystemIdFixture.Id;
            var systemIdListFixture = new SystemIdList()
            {
                SystemIds = new List<SystemId>() { jigsawSystemIdFixture }
            };
            var systemIds = systemIdListFixture.ToJson();
            var userToken = _fixture.Create<string>();
            var response = await _classUnderTest.Execute(systemIds, userToken, null, null, 0);
            Assert.AreEqual(SystemId.UnauthorisedMessage, response.SystemIds[^1].Error);
            _mockGetJigsawNotesUseCase.Verify(x =>
                x.Execute(customerIdFixture, null), Times.Never);
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
            response.SystemIds[^1].Error.Should().BeEquivalentTo(SystemId.NotFoundMessage);
        }
    }
}
