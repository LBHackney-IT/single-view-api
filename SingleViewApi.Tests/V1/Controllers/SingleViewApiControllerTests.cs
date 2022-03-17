using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;

namespace SingleViewApi.Tests.V1.Controllers
{
    [TestFixture]
    public class SingleViewApiControllerTests : LogCallAspectFixture
    {
        private SingleViewApiController _classUnderTest;
        private Mock<IGetByIdUseCase> _mockGetByIdUseCase;
        private Mock<IGetAllUseCase> _mockGetByAllUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetByIdUseCase = new Mock<IGetByIdUseCase>();
            _mockGetByAllUseCase = new Mock<IGetAllUseCase>();
            _classUnderTest = new SingleViewApiController(_mockGetByAllUseCase.Object, _mockGetByIdUseCase.Object);
        }


        //Add Tests Here
    }
}
