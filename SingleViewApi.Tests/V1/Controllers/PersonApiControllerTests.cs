using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.Controllers;

[TestFixture]
public class PersonApiControllerTests : LogCallAspectFixture
{
    [SetUp]
    public void SetUp()
    {
        _getPersonApiByIdUseCaseUseCaseMock = new Mock<IGetPersonApiByIdUseCase>();
        _classUnderTest = new PersonApiController(_getPersonApiByIdUseCaseUseCaseMock.Object);
    }

    private PersonApiController _classUnderTest;
    private Mock<IGetPersonApiByIdUseCase> _getPersonApiByIdUseCaseUseCaseMock;

    [Test]
    public void UseCaseGetsCalled()
    {
        const string id = "test-id";
        const string hackneyToken = "test-token";

        _ = _classUnderTest.GetPersonApiCustomer(id, hackneyToken);

        _getPersonApiByIdUseCaseUseCaseMock.Verify(x => x.Execute(id, hackneyToken), Times.Once);
    }
}
