using System.Linq;
using AutoFixture;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.Tests.V1.Controllers
{
    [TestFixture]
    public class NotesControllerTests : LogCallAspectFixture
    {
        private NotesController _classUnderTest;
        private Mock<IGetAllNotesByIdUseCase> _mockGetAllNotesByIdUseCase;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGetAllNotesByIdUseCase = new Mock<IGetAllNotesByIdUseCase>();
            _classUnderTest = new NotesController(_mockGetAllNotesByIdUseCase.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void UseCaseGetsCalled()
        {
            var systemIds = new SystemIdList() { SystemIds = _fixture.CreateMany<SystemId>().ToList() }.ToJson();
            var authorization = _fixture.Create<string>();

            _ = _classUnderTest.ListNotes(systemIds, authorization);

            _mockGetAllNotesByIdUseCase.Verify(x =>
                x.Execute(systemIds, authorization, null, 0), Times.Once);
        }
    }
}
