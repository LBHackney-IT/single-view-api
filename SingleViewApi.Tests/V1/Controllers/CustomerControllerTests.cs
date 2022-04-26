using System.Threading.Tasks;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;

namespace SingleViewApi.Tests.V1.Controllers
{
    [TestFixture]
    public class CustomerControllerTests : LogCallAspectFixture
    {
        private CustomerController _classUnderTest;
        private Mock<IGetCustomerByIdUseCase> _mockGetCustomerByIdUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetCustomerByIdUseCase = new Mock<IGetCustomerByIdUseCase>();
            _classUnderTest = new CustomerController(_mockGetCustomerByIdUseCase.Object);
        }


        //Add Tests Here

        [Test]
        public void UseCaseGetsCalled()
        {
            const string id = "test-id";
            const string token = "token";

            var result = _classUnderTest.GetCustomer(id, token);

            _mockGetCustomerByIdUseCase.Verify(x => x.Execute(id, token), Times.Once);
        }
    }
}
