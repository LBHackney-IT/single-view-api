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
            const string token = "token";

var results = _classUnderTest.SearchBySearchText(searchText, token);

            _mockGetSearchResultsBySearchTextUseCase.Verify(x => x.Execute(searchText, token), Times.Once);
        }
    }
}
