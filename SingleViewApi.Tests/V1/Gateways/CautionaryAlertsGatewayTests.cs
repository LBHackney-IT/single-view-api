using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SingleViewApi.Tests.V1.Gateways;

public class CautionaryAlertsGatewayTests
{
    private readonly Fixture _fixture = new Fixture();
    private CautionaryAlertsGateway _classUnderTest;
    private MockHttpMessageHandler _mockHttp;

    [SetUp]
    public void Setup()
    {
        _mockHttp = new MockHttpMessageHandler();
        var mockHttpClient = _mockHttp.ToHttpClient();
        const string baseUrl = "https://cautionary-alerts.api";

        _classUnderTest = new CautionaryAlertsGateway(mockHttpClient, baseUrl);
    }

    [Test]
    public void ARequestIsMade()
    {
        // Arrange
        const string id = "0123";
        const string userToken = "User token";

        // Act
        _ = _classUnderTest.GetCautionaryAlertsById(id, userToken).Result;

        // Assert
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Test]
    public async Task DataFromApiIsRetrieved()
    {
        // Arrange
        const string id = "0123";
        const string userToken = "User token";
        var rawJson = @"{
    'personId': '8d027289-0f0e-56a2-6473-980544f46b29',
        'alerts': [
        {
            'dateModified': '2020-03-24',
            'modifiedBy': 'GoogleSheet',
            'startDate': '2020-03-24',
            'endDate': null,
            'alertCode': 'VA',
            'description': 'Verbal Abuse',
            'reason': 'Tenant was abusive'
        }
        ]
    }";
        _mockHttp.Expect($"https://cautionary-alerts.api/cautionary-alerts/persons/{id}")
            .WithHeaders("Authorization", userToken)
            .Respond("application/json",
                rawJson);

        // Act
        var cautionaryAlerts = await _classUnderTest.GetCautionaryAlertsById(id, userToken);

        // Assert
        _mockHttp.VerifyNoOutstandingExpectation();
        Assert.AreEqual("2020-03-24", cautionaryAlerts.Alerts[0].DateModified);
        Assert.AreEqual("GoogleSheet", cautionaryAlerts.Alerts[0].ModifiedBy);
        Assert.AreEqual("2020-03-24", cautionaryAlerts.Alerts[0].StartDate);
        Assert.IsNull(cautionaryAlerts.Alerts[0].EndDate);
        Assert.AreEqual("VA", cautionaryAlerts.Alerts[0].AlertCode);
        Assert.AreEqual("Verbal Abuse", cautionaryAlerts.Alerts[0].Description);
        Assert.AreEqual("Tenant was abusive", cautionaryAlerts.Alerts[0].Reason);


    }

    [Test]
    public async Task GetCautionaryAlertsByIdReturnsNullIfApiIsNotResponding()
    {
        // Arrange
        const string id = "0123";
        const string userToken = "User token";
        var stubbedCautionaryAlerts = _fixture.Create<List<CautionaryAlert>>();

        _mockHttp.Expect($"https://cautionary-alerts.api/cautionary-alerts/persons/{id}")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.ServiceUnavailable, x => new StringContent(id));

        // Act
        var cautionaryAlerts = await _classUnderTest.GetCautionaryAlertsById(id, userToken);

        // Assert
        cautionaryAlerts.Should().BeNull();
    }

    [Test]
    public async Task GetCautionaryAlertsByIdReturnsNullIfEntityDoesNotExist()
    {
        // Arrange
        const string id = "0123";
        const string userToken = "User token";
        var stubbedCautionaryAlerts = _fixture.Create<List<CautionaryAlert>>();

        _mockHttp.Expect($"https://cautionary-alerts.api/cautionary-alerts/persons/{id}")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.NotFound, x => new StringContent(id));

        // Act
        var cautionaryAlerts = await _classUnderTest.GetCautionaryAlertsById(id, userToken);

        // Assert
        cautionaryAlerts.Should().BeNull();
    }

    [Test]
    public async Task GetCautionaryAlertsByIdReturnsNullIfUserNotAuthorised()
    {
        // Arrange
        const string id = "0123";
        const string userToken = "User token";
        var stubbedCautionaryAlerts = _fixture.Create<List<CautionaryAlert>>();

        _mockHttp.Expect($"https://y4xnj7mcpa.execute-api.eu-west-2.amazonaws.com/staging/api/v1/cautionary-alerts/persons/{id}")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.Unauthorized, x => new StringContent(id));

        // Act
        var cautionaryAlerts = await _classUnderTest.GetCautionaryAlertsById(id, userToken);

        // Assert
        cautionaryAlerts.Should().BeNull();
    }
}
