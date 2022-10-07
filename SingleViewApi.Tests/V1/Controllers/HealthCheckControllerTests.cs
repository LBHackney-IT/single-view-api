using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SingleViewApi.V1.Controllers;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.Controllers;

[TestFixture]
public class HealthCheckControllerTests
{
    [SetUp]
    public void SetUp()
    {
        _classUnderTest = new HealthCheckController();
    }

    private HealthCheckController _classUnderTest;

    [Test]
    public void ReturnsResponseWithStatus()
    {
        var expected = new Dictionary<string, object> { { "success", true } };
        var response = _classUnderTest.HealthCheck() as OkObjectResult;

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(200);
        response.Value.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void ThrowErrorThrows()
    {
        Assert.Throws<TestOpsErrorException>(_classUnderTest.ThrowError);
    }
}
