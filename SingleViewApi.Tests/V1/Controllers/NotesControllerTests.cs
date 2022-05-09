using System;
using System.Linq;
using AutoFixture;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Testing.Shared;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Request;

namespace SingleViewApi.Tests.V1.Controllers
{
    [TestFixture]
    public class NotesControllerTests : LogCallAspectFixture
    {
        private NotesController _classUnderTest;
        private Mock<IGetAllNotesByIdUseCase> _mockGetAllNotesByIdUseCase;
        private Mock<ICreateNoteUseCase> _mockCreateNoteUseCase;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGetAllNotesByIdUseCase = new Mock<IGetAllNotesByIdUseCase>();
            _mockCreateNoteUseCase = new Mock<ICreateNoteUseCase>();
            _classUnderTest = new NotesController(_mockGetAllNotesByIdUseCase.Object, _mockCreateNoteUseCase.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void GetAllNotesByIdUseCaseGetsCalled()
        {
            var systemIds = new SystemIdList() { SystemIds = _fixture.CreateMany<SystemId>().ToList() }.ToJson();
            var authorization = _fixture.Create<string>();

            _ = _classUnderTest.ListNotes(systemIds, authorization);

            _mockGetAllNotesByIdUseCase.Verify(x =>
                x.Execute(systemIds, authorization, null, 0), Times.Once);
        }

        [Test]
        public void CreateNoteUseCaseGetsCalled()
        {
            var authorization = _fixture.Create<string>();
            var createNoteRequest = _fixture.Create<CreateNoteRequest>();

            Console.WriteLine(JsonConvert.SerializeObject(createNoteRequest));

            _ = _classUnderTest.CreateNote(createNoteRequest, authorization);

            _mockCreateNoteUseCase.Verify(x =>
                x.Execute(createNoteRequest, authorization), Times.Once);
        }
    }
}