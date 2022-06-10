using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using SingleViewApi.V1.UseCase;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetNotesUseCaseTests : LogCallAspectFixture
    {
        private Mock<INotesGateway> _mockNotesGateway;
        private GetNotesUseCase _classUnderTest;
        private Fixture _fixture;
        private Mock<IDataSourceGateway> _mockDataSourceGateway;

        [SetUp]
        public void SetUp()
        {
            _mockNotesGateway = new Mock<INotesGateway>();
            _mockDataSourceGateway = new Mock<IDataSourceGateway>();
            _classUnderTest = new GetNotesUseCase(_mockNotesGateway.Object, _mockDataSourceGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetsNotes()
        {
            var tokenFixture = _fixture.Create<string>();
            var idFixture = _fixture.Create<string>();
            var notesFixture = _fixture.CreateMany<NotesApiResponseObject>().ToList();
            var stubbedDataSource = _fixture.Create<DataSource>();

            _mockNotesGateway.Setup(x =>
                x.GetAllById(idFixture, tokenFixture, null, 0)).ReturnsAsync(notesFixture);
            _mockDataSourceGateway.Setup(x => x.GetEntityById(1)).Returns(stubbedDataSource);

            var response = await _classUnderTest.Execute(idFixture, tokenFixture, null, 0);
            Assert.AreEqual(stubbedDataSource, response[^1].DataSource);
            Assert.AreEqual(notesFixture[^1].Id.ToString(), response[^1].DataSourceId);
            Assert.AreEqual(notesFixture[^1].Description, response[^1].Description);
        }

        [Test]
        public async Task ReturnsNullIfGatewayErrors()
        {
            var tokenFixture = _fixture.Create<string>();
            var idFixture = _fixture.Create<string>();

            _mockNotesGateway.Setup(x =>
                x.GetAllById(idFixture, tokenFixture, null, 0)).ReturnsAsync((List<NotesApiResponseObject>) null);

            var response = await _classUnderTest.Execute(idFixture, tokenFixture, null, 0);
            Assert.IsNull(response);
        }
    }
}
