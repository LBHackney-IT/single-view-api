using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.Controllers
{
    public class AcademyControllerTests : LogCallAspectFixture
    {
        private AcademyController _classUnderTest;
        private Mock<IGetCouncilTaxAccountByAccountRefUseCase> _mockGetCouncilTaxAccountByAccountRefUseCase;


        [SetUp]
        public void SetUp()
        {
            _mockGetCouncilTaxAccountByAccountRefUseCase = new Mock<IGetCouncilTaxAccountByAccountRefUseCase>();
            _classUnderTest = new AcademyController(_mockGetCouncilTaxAccountByAccountRefUseCase.Object);
        }


        [Test]
        public void UseCaseGetsCalled()
        {
            const string authorization = "token";
            const string id = "Testid";


            _ = _classUnderTest.GetCouncilTaxAccount(id, authorization);

            _mockGetCouncilTaxAccountByAccountRefUseCase.Verify(x => x.Execute(id, authorization), Times.Once);
        }
    }
}
