using System.Linq;
using AutoFixture;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Factories;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetAllUseCaseTests : LogCallAspectFixture
    {
        private Mock<IDataSourceGateway> _mockGateway;
        private GetAllUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IDataSourceGateway>();
            _classUnderTest = new GetAllUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void GetsAllFromTheGateway()
        {
            var stubbedEntities = _fixture.CreateMany<DataSource>().ToList();
            _mockGateway.Setup(x => x.GetAll()).Returns(stubbedEntities);

            var expectedResponse = new ResponseObjectList { ResponseObjects = stubbedEntities.ToResponse() };

            _classUnderTest.Execute().Should().BeEquivalentTo(expectedResponse);
        }

        //TODO: Add extra tests here for extra functionality added to the use case
    }
}
