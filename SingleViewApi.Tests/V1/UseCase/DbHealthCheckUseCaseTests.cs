using System.Threading;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.HealthChecks;
using Moq;
using NUnit.Framework;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase;

[TestFixture]
public class DbHealthCheckUseCaseTests
{
    [SetUp]
    public void SetUp()
    {
        _description = _faker.Random.Words();

        _mockHealthCheckService = new Mock<IHealthCheckService>();
        var compositeHealthCheckResult = new CompositeHealthCheckResult(CheckStatus.Healthy);
        compositeHealthCheckResult.Add("test", CheckStatus.Healthy, _description);


        _mockHealthCheckService.Setup(s =>
                s.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(compositeHealthCheckResult);

        _classUnderTest = new DbHealthCheckUseCase(_mockHealthCheckService.Object);
    }

    private Mock<IHealthCheckService> _mockHealthCheckService;
    private DbHealthCheckUseCase _classUnderTest;

    private readonly Faker _faker = new();
    private string _description;

    [Test]
    public void ReturnsResponseWithStatus()
    {
        var response = _classUnderTest.Execute();

        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Message.Should().BeEquivalentTo("test: " + _description);
    }
}
