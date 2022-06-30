using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using JsonSerializer = System.Text.Json.JsonSerializer;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways;

[TestFixture]
public class AcademyGatewayTests
{
    private Fixture _fixture;
    private AcademyGateway _classUnderTest;
    private MockHttpMessageHandler _mockHttp;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _mockHttp = new MockHttpMessageHandler();
        const string baseUrl = "https://academy.api";
        var mockClient = _mockHttp.ToHttpClient();
        _classUnderTest = new AcademyGateway(mockClient, baseUrl);

    }

    [Test]
    public void ARequestIsMadeToSearch()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"https://academy.api/council-tax/search?firstName={firstName}&lastName={lastName}")
            .WithHeaders("Authorization", userToken);

        _ = _classUnderTest.GetCouncilTaxAccountsByCustomerName(firstName, lastName, userToken);

        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Test]
    public async Task SearchEndpointReturnsNullIfCustomerDoesNotExist()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"https://academy.api/council-tax/search?firstName={firstName}&lastName={lastName}")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.NotFound);

        var results = await _classUnderTest.GetCouncilTaxAccountsByCustomerName(firstName, lastName, userToken);

        results.Should().BeNull();
    }

    [Test]
    public async Task SearchResponseObjectIsReturnedFromSearch()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedResponse = _fixture.Create<CouncilTaxSearchResponseObject>();

        _mockHttp.Expect($"https://academy.api/council-tax/search?firstName={firstName}&lastName={lastName}")
            .WithHeaders("Authorization", userToken)
            .Respond("application/json",
                JsonSerializer.Serialize<CouncilTaxSearchResponseObject>(stubbedResponse));

        var results = await _classUnderTest.GetCouncilTaxAccountsByCustomerName(firstName, lastName, userToken);

        _mockHttp.VerifyNoOutstandingExpectation();
        Assert.AreEqual(stubbedResponse.Customers[0].Id, results.Customers[0].Id);
        Assert.AreEqual(stubbedResponse.Customers[0].FirstName, results.Customers[0].FirstName);
        Assert.AreEqual(stubbedResponse.Customers[0].LastName, results.Customers[0].LastName);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Line1, results.Customers[0].FullAddress.Line1);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Line2, results.Customers[0].FullAddress.Line2);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Line3, results.Customers[0].FullAddress.Line3);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Postcode, results.Customers[0].FullAddress.Postcode);

    }
}
