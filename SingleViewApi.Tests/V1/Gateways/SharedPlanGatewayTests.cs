using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways;

public class SharedPlanGatewayTests
{
    private MockHttpMessageHandler _mockHttp;
    private string _baseUrl;
    private SharedPlanGateway _classUnderTest;
    private GetSharedPlanRequest _sharedPlanRequest;
    private string _xApiKey;

    [SetUp]
    public void Setup()
    {
        _mockHttp = new MockHttpMessageHandler();
        var mockHttpClient = _mockHttp.ToHttpClient();
        _baseUrl = "https://shared-plan.api";
        _xApiKey = "x-api-key";

        _classUnderTest = new SharedPlanGateway(mockHttpClient, _baseUrl, _xApiKey);

        _sharedPlanRequest = new GetSharedPlanRequest()
        {
            FirstName = "John",
            LastName = "Smith",
            SystemIds = new List<string>() {"SingleView", "Jigsaw", "Housing",}
        };
    }

    [Test]
    public void ARequestIsMade()
    {
        // Act
        _classUnderTest.GetSharedPlans(_sharedPlanRequest);

        // Assert
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Test]
    public async Task DataFromApiIsRetrieved()
    {
        // Arrange
        var data = new SharedPlanResponseObject {PlanIds = new List<string> {"101010", "202020", "303030"}};
        _mockHttp.Expect($"{_baseUrl}/api/plans/find")
            .WithHeaders("x-api-key", _xApiKey)
            .Respond("application/json", JsonConvert.SerializeObject(data));

        // Act
        var sharedPlanIds = await _classUnderTest.GetSharedPlans(_sharedPlanRequest);

        // Assert
        sharedPlanIds.PlanIds.Count.Should().Be(3);
        sharedPlanIds.PlanIds[0].Should().BeEquivalentTo("101010");
        sharedPlanIds.PlanIds[1].Should().BeEquivalentTo("202020");
        sharedPlanIds.PlanIds[2].Should().BeEquivalentTo("303030");
    }

    [Test]
    public async Task GetSharedPlansReturnsNullIfApiIsNotResponding()
    {
        // Arrange
        _mockHttp.Expect($"{_baseUrl}/api/plans/find")
            .WithHeaders("x-api-key", _xApiKey)
            .Respond(HttpStatusCode.ServiceUnavailable);

        // Act
        var sharedPlanIds = await _classUnderTest.GetSharedPlans(_sharedPlanRequest);

        // Assert
        sharedPlanIds.Should().BeNull();
    }

    [Test]
    public async Task GetSharedPlansReturnsNullIfSharedPlansDontExist()
    {
        // Arrange
        _mockHttp.Expect($"{_baseUrl}/api/plans/find")
            .WithHeaders("x-api-key", _xApiKey)
            .Respond(HttpStatusCode.NotFound);

        // Act
        var sharedPlanIds = await _classUnderTest.GetSharedPlans(_sharedPlanRequest);

        // Assert
        sharedPlanIds.Should().BeNull();
    }

    [Test]
    public async Task GetSharedPlansReturnsNullIfUserNotAuthorised()
    {
        // Arrange
        _mockHttp.Expect($"{_baseUrl}/api/plans/find")
            .WithHeaders("x-api-key", _xApiKey)
            .Respond(HttpStatusCode.Unauthorized);

        // Act
        var sharedPlanIds = await _classUnderTest.GetSharedPlans(_sharedPlanRequest);

        // Assert
        sharedPlanIds.Should().BeNull();
    }
}
