using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.Controllers;

[TestFixture]
public class JigsawCasesControllerTests : LogCallAspectFixture
{
    [SetUp]
    public void SetUp()
    {
        _getJigsawCasesByCustomerIdUseCaseMock = new Mock<IGetJigsawCasesByCustomerIdUseCase>();
        _classUnderTest = new JigsawCasesController(_getJigsawCasesByCustomerIdUseCaseMock.Object);
    }

    private JigsawCasesController _classUnderTest;
    private Mock<IGetJigsawCasesByCustomerIdUseCase> _getJigsawCasesByCustomerIdUseCaseMock;

    [Test]
    public void UseCaseGetsCalled()
    {
        const string id = "test-id";
        const string redisId = "test-redis-id";
        const string hackneyToken = "test-hackney-token";

        _ = _classUnderTest.GetCasesByCustomerId(id, redisId, hackneyToken);

        _getJigsawCasesByCustomerIdUseCaseMock.Verify(x => x.Execute(id, redisId, hackneyToken), Times.Once);
    }
}
