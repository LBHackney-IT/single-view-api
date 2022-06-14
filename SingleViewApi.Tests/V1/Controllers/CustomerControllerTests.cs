using System.Threading.Tasks;
using AutoFixture;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.Tests.V1.Controllers
{
    [TestFixture]
    public class CustomerControllerTests : LogCallAspectFixture
    {
        private CustomerController _classUnderTest;
        private Mock<IGetPersonApiByIdUseCase> _mockGetCustomerByIdUseCase;
        private Mock<ICreateCustomerUseCase> _mockCreateCustomerUseCase;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGetCustomerByIdUseCase = new Mock<IGetPersonApiByIdUseCase>();
            _mockCreateCustomerUseCase = new Mock<ICreateCustomerUseCase>();
            _classUnderTest = new CustomerController(_mockGetCustomerByIdUseCase.Object, _mockCreateCustomerUseCase.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void UseCaseGetsCalled()
        {
            const string id = "test-id";
            const string token = "token";

            _classUnderTest.GetCustomer(id, token);

            _mockGetCustomerByIdUseCase.Verify(x => x.Execute(id, token), Times.Once);
        }

        [Test]
        public void CreateUseCaseGetsCalled()
        {
            var request = _fixture.Create<CreateCustomerRequest>();

            _mockCreateCustomerUseCase.Setup(x => x.Execute(request)).Returns(_fixture.Create<SavedCustomer>());

            _ = _classUnderTest.SaveCustomer(request);

            _mockCreateCustomerUseCase.Verify(x => x.Execute(request), Times.Once);
        }
    }
}
