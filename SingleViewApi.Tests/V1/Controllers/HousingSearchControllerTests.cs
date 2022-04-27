using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.Controllers
{
    public class HousingSearchControllerTests
    {
        private HousingSearchController _classUnderTest;
        private Mock<IGetSearchResultsBySearchTextUseCase> _mockGetSearchResultsBySearchTextUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetSearchResultsBySearchTextUseCase = new Mock<IGetSearchResultsBySearchTextUseCase>();
            _classUnderTest = new HousingSearchController(_mockGetSearchResultsBySearchTextUseCase.Object);
        }


        [Test]
        public void UseCaseGetsCalled()
        {
            const string searchText = "testSearch";
            const string authorization = "token";
            const int page = 1;

            var results = _classUnderTest.SearchBySearchText(searchText, page, authorization);

            _mockGetSearchResultsBySearchTextUseCase.Verify(x => x.Execute(searchText, page, authorization), Times.Once);
        }
    }
}
