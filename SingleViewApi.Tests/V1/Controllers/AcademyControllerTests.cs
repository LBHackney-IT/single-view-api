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
        private Mock<IGetCouncilTaxAccountByIdUseCase> _mockGetCouncilTaxAccountByIdUseCase;


        [SetUp]
        public void SetUp()
        {
            _mockGetCouncilTaxAccountByIdUseCase = new Mock<IGetCouncilTaxAccountByIdUseCase>();
            _classUnderTest = new AcademyController(_mockGetCouncilTaxAccountByIdUseCase.Object);
        }


        [Test]
        public void UseCaseGetsCalled()
        {
            const string authorization = "token";
            const string id = "Testid";


            _ = _classUnderTest.GetCouncilTaxAccount(id, authorization);

            _mockGetCouncilTaxAccountByIdUseCase.Verify(x => x.Execute(id, authorization), Times.Once);
        }
    }
}
