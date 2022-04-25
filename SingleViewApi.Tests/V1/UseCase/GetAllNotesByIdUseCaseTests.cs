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
    public class GetAllNotesByIdUseCaseTests : LogCallAspectFixture
    {
        private Mock<INotesGateway> _mockGateway;
        private GetAllUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<INotesGateway>();
            _classUnderTest = new GetAllNotesById(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void GetsAllFromTheGateway()
        {
            var stubbedEntities = _fixture.CreateMany<Entity>().ToList();
            _mockGateway.Setup(x => x.GetAll()).Returns(stubbedEntities);

            var expectedResponse = new ResponseObjectList { ResponseObjects = stubbedEntities.ToResponse() };

            _classUnderTest.Execute().Should().BeEquivalentTo(expectedResponse);
        }

        //TODO: Add extra tests here for extra functionality added to the use case
    }
}
