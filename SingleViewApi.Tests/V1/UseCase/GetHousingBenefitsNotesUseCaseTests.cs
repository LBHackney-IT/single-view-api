using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using SingleViewApi.V1.UseCase;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.Tests.V1.UseCase
{
    public class GetHousingBenefitsNotesUseCaseTests : LogCallAspectFixture
    {
        private Mock<IAcademyGateway> _mockAcademyGateway;
        private GetHousingBenefitsNotesUseCase _classUnderTest;
        private Fixture _fixture;
        private Mock<IDataSourceGateway> _mockDataSourceGateway;

        [SetUp]
        public void SetUp()
        {
            _mockAcademyGateway = new Mock<IAcademyGateway>();
            _mockDataSourceGateway = new Mock<IDataSourceGateway>();
            _classUnderTest =
                new GetHousingBenefitsNotesUseCase(_mockAcademyGateway.Object, _mockDataSourceGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetsNotes()
        {
            var idFixture = _fixture.Create<string>();
            var hackneyTokenFixture = _fixture.Create<string>();
            var notesFixture = new List<AcademyNotesResponseObject>()
            {
                new AcademyNotesResponseObject()
                {
                    Date = "31.03.2022 15:39:37  1017585577",
                    UserId = "Diane",
                    Note = "Lorem ipsum dolor sit amet."
                }
            };
            var stubbedDataSourceName = _fixture.Create<string>();

            _mockDataSourceGateway.Setup(x => x.GetEntityById(4)).Returns(new DataSource()
            {
                Id = 2,
                Name = stubbedDataSourceName
            });

            _mockAcademyGateway.Setup(x =>
                x.GetHousingBenefitsNotes(idFixture, hackneyTokenFixture)).ReturnsAsync(notesFixture);

            var response = await _classUnderTest.Execute(idFixture, hackneyTokenFixture);

            Assert.AreEqual(notesFixture[^1].Note, response[^1].Description);
            Assert.AreEqual(DateTime.Parse("2022-03-31 15:39:37"), response[^1].CreatedAt);
            Assert.AreEqual("Academy Housing Benefits Note", response[^1].Categorisation.Description);
            Assert.AreEqual(notesFixture[^1].UserId, response[^1].Author.FullName);
            Assert.AreEqual(stubbedDataSourceName, response[^1].DataSource);
        }
    }
}
