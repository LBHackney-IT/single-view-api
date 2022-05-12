using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.Controllers
{
    public class StoreJigsawCredentialsControllerTests
    {
        private StoreJigsawCredentialsController _classUnderTest;
        private Mock<IStoreJigsawCredentialsUseCase> _mockStoreJigsawCredentialsUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockStoreJigsawCredentialsUseCase = new Mock<IStoreJigsawCredentialsUseCase>();
            _classUnderTest = new StoreJigsawCredentialsController(_mockStoreJigsawCredentialsUseCase.Object);
        }


        [Test]
        public void UseCaseGetsCalled()
        {
            const string jwt = "Fake-jwt";

            _ = _classUnderTest.StoreJigsawCredentials(jwt);

            _mockStoreJigsawCredentialsUseCase.Verify(x => x.Execute(jwt), Times.Once);
        }
    }
}

