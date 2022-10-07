using AutoFixture;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.Tests.V1.Controllers;

public class SharedPlanControllerTests : LogCallAspectFixture
{
    private SharedPlanController _classUnderTest;
    private Fixture _fixture;
    private Mock<ICreateSharedPlanUseCase> _mockCreateSharedPlanUseCase;

    [SetUp]
    public void SetUp()
    {
        _mockCreateSharedPlanUseCase = new Mock<ICreateSharedPlanUseCase>();
        _classUnderTest = new SharedPlanController(_mockCreateSharedPlanUseCase.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void CreateSharedPlanGetsCalled()
    {
        var request = _fixture.Create<CreateSharedPlanRequest>();
        var response = _fixture.Create<CreateSharedPlanResponseObject>();

        _mockCreateSharedPlanUseCase.Setup(x =>
                x.Execute(It.IsAny<CreateSharedPlanRequest>()))
            .ReturnsAsync(response);

        var x = _classUnderTest.CreateSharedPlan(request);

        _mockCreateSharedPlanUseCase.Verify(x =>
            x.Execute(It.IsAny<CreateSharedPlanRequest>()), Times.Once);
    }
}
