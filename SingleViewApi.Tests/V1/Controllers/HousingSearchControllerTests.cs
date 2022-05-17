using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.Controllers
{
    public class HousingSearchControllerTests
    {
        private HousingSearchController _classUnderTest;
        private Mock<IGetSearchResultsByNameUseCase> _mockGetSearchResultsByNameUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetSearchResultsByNameUseCase = new Mock<IGetSearchResultsByNameUseCase>();
            _classUnderTest = new HousingSearchController(_mockGetSearchResultsByNameUseCase.Object);
        }


        [Test]
        public void UseCaseGetsCalled()
        {
            const string firstName = "Test";
            const string lastName = "Test";
            const string authorization = "token";
            const int page = 1;

            var results = _classUnderTest.SearchBySearchText(firstName, lastName, page, authorization);

            _mockGetSearchResultsByNameUseCase.Verify(x => x.Execute(firstName, lastName, page, authorization), Times.Once);
        }
    }
}
