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
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetAllNotesUseCaseTests : LogCallAspectFixture
    {
        private Mock<IGetNotesUseCase> _mockGetNotesUseCase;
        private Mock<IGetJigsawNotesUseCase> _mockGetJigsawNotesUseCase;
        private GetAllNotesUseCase _classUnderTest;
        private Fixture _fixture;
        private Mock<IDataSourceGateway> _mockDataSourceGateway;


        [SetUp]
        public void SetUp()
        {
            _mockGetNotesUseCase = new Mock<IGetNotesUseCase>();
            _mockGetJigsawNotesUseCase = new Mock<IGetJigsawNotesUseCase>();
            _mockDataSourceGateway = new Mock<IDataSourceGateway>();

            _classUnderTest = new GetAllNotesUseCase(_mockGetNotesUseCase.Object, _mockGetJigsawNotesUseCase.Object, _mockDataSourceGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetsAllNotes()
        {
            var stubbedJigsawDataSource = _fixture.Create<DataSource>();
            var stubbedHousingSearchDataSource = _fixture.Create<DataSource>();

            var personApiSystemIdFixture = _fixture.Build<SystemId>()
                .With(o => o.SystemName, stubbedHousingSearchDataSource.Name).Create();
            var personApiSystemId = personApiSystemIdFixture.Id;
            var jigsawSystemIdFixture = _fixture.Build<SystemId>()
                .With(o => o.SystemName, stubbedJigsawDataSource.Name).Create();
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

            _mockDataSourceGateway.Setup(x => x.GetEntityById(2)).Returns(stubbedJigsawDataSource);

            var notesFixture = new List<NoteResponseObject>();
            var notesApiNoteResponseObjectListFixture = _fixture.Build<NoteResponseObject>()
                .With(o => o.DataSource, stubbedHousingSearchDataSource).CreateMany().ToList();
            var jigsawNoteResponseObjectListFixture = _fixture.Build<NoteResponseObject>()
                .With(o => o.DataSource, stubbedJigsawDataSource).CreateMany().ToList();
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
                x.Execute(jigsawSystemId, redisKey, userToken)).ReturnsAsync(jigsawNoteResponseObjectListFixture);

            var response = await _classUnderTest.Execute(systemIds, userToken, redisKey, paginationToken, pageSize);
            // Assert.AreEqual(systemIdListFixture.SystemIds[^1].Id, response.SystemIds[^1].Id);
            Assert.AreEqual(notesResponseFixture.Notes.Count, response.Notes.Count);
            // Assert.AreEqual(notesResponseFixture.Notes[^1].Id, response.Notes[^1].Id);
        }

        [Test]
        public async Task IgnoresJigsawNotesIfRedisKeyIsNull()
        {
            var stubbedJigsawDataSource = _fixture.Create<DataSource>();

            var jigsawSystemIdFixture = _fixture.Build<SystemId>()
                .With(o => o.SystemName, stubbedJigsawDataSource.Name).Create();

            _mockDataSourceGateway.Setup(x => x.GetEntityById(2)).Returns(stubbedJigsawDataSource);

            var customerIdFixture = jigsawSystemIdFixture.Id;
            var hackneyTokenFixture = _fixture.Create<string>();

            var systemIdListFixture = new SystemIdList()
            {
                SystemIds = new List<SystemId>() { jigsawSystemIdFixture }
            };
            var systemIds = systemIdListFixture.ToJson();
            var userToken = _fixture.Create<string>();
            var response = await _classUnderTest.Execute(systemIds, userToken, null, null, 0);
            Assert.AreEqual(SystemId.UnauthorisedMessage, response.SystemIds[^1].Error);
            _mockGetJigsawNotesUseCase.Verify(x =>
                x.Execute(customerIdFixture, null, hackneyTokenFixture), Times.Never);
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
            var hackneyTokenFixture = _fixture.Create<string>();
            var stubbedJigsawDataSource = _fixture.Create<DataSource>();

            _mockDataSourceGateway.Setup(x => x.GetEntityById(2)).Returns(stubbedJigsawDataSource);

            _mockGetNotesUseCase.Setup(x =>
                x.Execute(id, userToken, paginationToken, pageSize)).ReturnsAsync((List<NoteResponseObject>) null);

            _mockGetJigsawNotesUseCase.Setup(x =>
                x.Execute(id, redisKey, hackneyTokenFixture)).ReturnsAsync((List<NoteResponseObject>) null);

            var response = await _classUnderTest.Execute(systemIds, userToken, redisKey, paginationToken, pageSize);
            response.SystemIds[^1].Id.Should().BeEquivalentTo(systemIdListFixture.SystemIds[^1].Id);
            response.SystemIds[^1].SystemName.Should().BeEquivalentTo(systemIdListFixture.SystemIds[^1].SystemName);
            response.SystemIds[^1].Error.Should().BeEquivalentTo(SystemId.NotFoundMessage);
        }
    }
}
