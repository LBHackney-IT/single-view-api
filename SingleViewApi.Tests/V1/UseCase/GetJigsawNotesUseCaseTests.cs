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
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetJigsawNotesUseCaseTests : LogCallAspectFixture
    {
        private Mock<IJigsawGateway> _mockJigsawGateway;
        private Mock<IGetJigsawAuthTokenUseCase> _mockGetJigsawAuthTokenUseCase;
        private GetJigsawNotesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockJigsawGateway = new Mock<IJigsawGateway>();
            _mockGetJigsawAuthTokenUseCase = new Mock<IGetJigsawAuthTokenUseCase>();
            _classUnderTest =
                new GetJigsawNotesUseCase(_mockJigsawGateway.Object, _mockGetJigsawAuthTokenUseCase.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetsNotes()
        {
            var redisKeyFixture = _fixture.Create<string>();
            var tokenFixture = _fixture.Create<string>();
            var idFixture = _fixture.Create<string>();
            var jigsawNotesFixture = _fixture.CreateMany<JigsawNotesResponseObject>().ToList();

            _mockGetJigsawAuthTokenUseCase.Setup(x =>
                x.Execute(redisKeyFixture)).ReturnsAsync(tokenFixture);

            _mockJigsawGateway.Setup(x =>
                x.GetCustomerNotesByCustomerId(idFixture, tokenFixture)).ReturnsAsync(jigsawNotesFixture);

            var response = await _classUnderTest.Execute(idFixture, redisKeyFixture);
            Assert.AreEqual(DataSource.Jigsaw, response[^1].DataSource);
            Assert.AreEqual(jigsawNotesFixture[^1].Id.ToString(), response[^1].DataSourceId);
            Assert.AreEqual(jigsawNotesFixture[^1].Content, response[^1].Description);
        }

        [Test]
        public async Task ReturnsNullIfGatewayErrors()
        {
            var redisKeyFixture = _fixture.Create<string>();
            var tokenFixture = _fixture.Create<string>();
            var idFixture = _fixture.Create<string>();

            _mockGetJigsawAuthTokenUseCase.Setup(x =>
                x.Execute(redisKeyFixture)).ReturnsAsync((string) null);

            _mockJigsawGateway.Setup(x =>
                x.GetCustomerNotesByCustomerId(idFixture, tokenFixture)).ReturnsAsync((List<JigsawNotesResponseObject>) null);

            var response = await _classUnderTest.Execute(idFixture, redisKeyFixture);
            Assert.IsNull(response);
        }
    }
}
