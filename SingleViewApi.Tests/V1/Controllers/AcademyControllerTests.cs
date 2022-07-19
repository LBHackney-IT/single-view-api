using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.Controllers
{
    public class AcademyControllerTests : LogCallAspectFixture
    {
        private AcademyController _classUnderTest;
        private Mock<IGetCouncilTaxAccountByAccountRefUseCase> _mockGetCouncilTaxAccountByAccountRefUseCase;
        private Mock<IGetHousingBenefitsAccountByAccountRefUseCase> _mockGetHousingBenefitsAccountByAccountRefUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetCouncilTaxAccountByAccountRefUseCase = new Mock<IGetCouncilTaxAccountByAccountRefUseCase>();
            _mockGetHousingBenefitsAccountByAccountRefUseCase = new Mock<IGetHousingBenefitsAccountByAccountRefUseCase>();
            _classUnderTest = new AcademyController(
                _mockGetCouncilTaxAccountByAccountRefUseCase.Object,
                _mockGetHousingBenefitsAccountByAccountRefUseCase.Object);
        }

        [Test]
        public void GetCouncilTaxAccountUseCaseExecutesOnce()
        {
            const string authorization = "token";
            const string id = "Testid";

            _ = _classUnderTest.GetCouncilTaxAccount(id, authorization);

            _mockGetCouncilTaxAccountByAccountRefUseCase.Verify(x =>
                x.Execute(id, authorization), Times.Once);
        }

        [Test]
        public void GetHousingBenefitsAccountUseCaseExecutesOnce()
        {
            const string authorization = "token";
            const string id = "Testid";

            _ = _classUnderTest.GetHousingBenefitsAccount(id, authorization);

            _mockGetHousingBenefitsAccountByAccountRefUseCase.Verify(x =>
                x.Execute(id, authorization), Times.Once);
        }
    }
}
