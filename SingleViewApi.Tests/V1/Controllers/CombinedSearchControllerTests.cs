using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.Controllers
{
    public class CombinedSearchControllerTests
    {
        private CombinedSearchController _classUnderTest;
        private Mock<IGetCombinedSearchResultsByNameUseCase> _mockGetCombinedSearchResultsByNameUseCase;


        [SetUp]
        public void SetUp()
        {
            _mockGetCombinedSearchResultsByNameUseCase = new Mock<IGetCombinedSearchResultsByNameUseCase>();
            _classUnderTest = new CombinedSearchController(_mockGetCombinedSearchResultsByNameUseCase.Object);
        }


        [Test]
        public void UseCaseGetsCalled()
        {
            const string firstName = "Test";
            const string lastName = "Test";
            const string authorization = "token";
            const string redisId = "Testid";
            const int page = 1;

            _ = _classUnderTest.SearchByName(firstName, lastName, page, redisId, authorization);

           _mockGetCombinedSearchResultsByNameUseCase.Verify(x => x.Execute(firstName, lastName, page, authorization, redisId), Times.Once);
        }
    }
}
