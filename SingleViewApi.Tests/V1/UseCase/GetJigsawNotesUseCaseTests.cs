using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using SingleViewApi.V1.UseCase;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetJigsawNotesUseCaseTests : LogCallAspectFixture
    {
        private Mock<IJigsawGateway> _mockJigsawGateway;
        private Mock<IGetJigsawAuthTokenUseCase> _mockGetJigsawAuthTokenUseCase;
        private GetJigsawNotesUseCase _classUnderTest;
        private Fixture _fixture;
        private Mock<IDataSourceGateway> _mockDataSourceGateway;
        private Mock<IGetJigsawActiveCaseNotesUseCase> _mockGetJigsawActiveCaseNotesUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockJigsawGateway = new Mock<IJigsawGateway>();
            _mockGetJigsawAuthTokenUseCase = new Mock<IGetJigsawAuthTokenUseCase>();
            _mockGetJigsawActiveCaseNotesUseCase = new Mock<IGetJigsawActiveCaseNotesUseCase>();
            _mockDataSourceGateway = new Mock<IDataSourceGateway>();
            _classUnderTest =
                new GetJigsawNotesUseCase(_mockJigsawGateway.Object, _mockGetJigsawAuthTokenUseCase.Object, _mockGetJigsawActiveCaseNotesUseCase.Object, _mockDataSourceGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetsNotes()
        {
            var redisKeyFixture = _fixture.Create<string>();
            var authGatewayResponseFixture = new AuthGatewayResponse()
            {
                Token = _fixture.Create<string>()
            };
            var idFixture = _fixture.Create<string>();
            var hackneyTokenFixture = _fixture.Create<string>();
            var jigsawNotesFixture = _fixture.CreateMany<JigsawNotesResponseObject>().ToList();
            var stubbedDataSourceName = _fixture.Create<string>();

            _mockDataSourceGateway.Setup(x => x.GetEntityById(2)).Returns(new DataSource()
            {
                Id = 2,
                Name = stubbedDataSourceName
            });

            _mockGetJigsawAuthTokenUseCase.Setup(x =>
                x.Execute(redisKeyFixture, hackneyTokenFixture)).ReturnsAsync(authGatewayResponseFixture);

            _mockJigsawGateway.Setup(x =>
                x.GetCustomerNotesByCustomerId(idFixture, authGatewayResponseFixture.Token)).ReturnsAsync(jigsawNotesFixture);

            var response = await _classUnderTest.Execute(idFixture, redisKeyFixture, hackneyTokenFixture);
            Assert.AreEqual(stubbedDataSourceName, response[^1].DataSource);
            Assert.AreEqual(jigsawNotesFixture[^1].Id.ToString(), response[^1].DataSourceId);
            Assert.AreEqual(jigsawNotesFixture[^1].Content, response[^1].Description);
        }

        [Test]
        public async Task ReturnsNullIfGatewayErrors()
        {
            var redisKeyFixture = _fixture.Create<string>();
            var idFixture = _fixture.Create<string>();
            var authGatewayResponseFixture = _fixture.Create<AuthGatewayResponse>();
            var hackneyTokenFixture = _fixture.Create<string>();

            _mockGetJigsawAuthTokenUseCase.Setup(x =>
                x.Execute(redisKeyFixture, hackneyTokenFixture)).ReturnsAsync(authGatewayResponseFixture);

            _mockJigsawGateway.Verify(x =>
                x.GetCustomerNotesByCustomerId(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            var response = await _classUnderTest.Execute(idFixture, redisKeyFixture, hackneyTokenFixture);
            Assert.IsNull(response);
        }
    }
}
