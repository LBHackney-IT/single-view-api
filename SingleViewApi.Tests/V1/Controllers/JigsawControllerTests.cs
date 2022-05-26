using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.Controllers;

[TestFixture]
public class JigsawControllerTests : LogCallAspectFixture
{
    private JigsawController _classUnderTest;
    private Mock<IGetJigsawCustomerByIdUseCase> _getJigsawCustomerByIdUseCaseMock;

    [SetUp]
    public void SetUp()
    {
        _getJigsawCustomerByIdUseCaseMock = new Mock<IGetJigsawCustomerByIdUseCase>();
        _classUnderTest = new JigsawController(_getJigsawCustomerByIdUseCaseMock.Object);
    }

    [Test]
    public void UseCaseGetsCalled()
    {
        const string id = "test-id";
        const string hackneyToken = "test-token";
        const string redisId = "test-redis-id";

        _ = _classUnderTest.GetCustomerById(id, redisId, hackneyToken);

        _getJigsawCustomerByIdUseCaseMock.Verify(x => x.Execute(id, redisId, hackneyToken), Times.Once);
    }
}
