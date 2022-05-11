using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using ServiceStack;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways;

public class JigsawGatewayTests
{
    private JigsawGateway _classUnderTest;
    private MockHttpMessageHandler _mockHttp;
    private string _authUrl;
    private string _searchUrl;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _authUrl = "https://api.jigsaw-auth";
        _searchUrl = "https://api.jigsaw-search";
        _mockHttp = new MockHttpMessageHandler();
        var mockClient = _mockHttp.ToHttpClient();
        _classUnderTest = new JigsawGateway(mockClient, _authUrl, _searchUrl);
    }

    [Test]
    public void ARequestIsMade()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_searchUrl}?search={firstName}%20{lastName}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}");

        _ = _classUnderTest.GetCustomers(firstName, lastName, bearerToken);

        _mockHttp.VerifyNoOutstandingExpectation();


    }
    [Test]
    public async Task GetCustomersReturnsNullIfThereAreNoResults() {

        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_searchUrl}?search={firstName}%20{lastName}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.NotFound);

        var searchResults = await _classUnderTest.GetCustomers(firstName, lastName, bearerToken);

        searchResults.Should().BeNull();
    }

    [Test]
    public async Task GetCustomersReturnsNullIfUnauthorised()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_searchUrl}?search={firstName}%20{lastName}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.Unauthorized);

        var searchResults = await _classUnderTest.GetCustomers(firstName, lastName, bearerToken);

        searchResults.Should().BeNull();
    }

    [Test]
    public async Task GetCustomersReturnsNullIfApiIsUnavailable()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_searchUrl}?search={firstName}%20{lastName}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.ServiceUnavailable);

        var searchResults = await _classUnderTest.GetCustomers(firstName, lastName, bearerToken);

        searchResults.Should().BeNull();
    }

    [Test]
    public async Task DataFromApiIsReturned()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();
        var searchResultsResponseObject = _fixture.CreateMany<JigsawCustomerSearchApiResponseObject>().ToList();

        _mockHttp.Expect($"{_searchUrl}?search={firstName}%20{lastName}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond("application/json", searchResultsResponseObject.ToJson());

        var searchResults = await _classUnderTest.GetCustomers(firstName, lastName, bearerToken);

        var searchResponseObject = searchResultsResponseObject[^1];
        var searchResponseObjectResult = searchResults[^1];

        Assert.AreEqual(searchResponseObject.Id, searchResponseObjectResult.Id);
        Assert.AreEqual(searchResponseObject.FirstName, searchResponseObjectResult.FirstName);
        Assert.AreEqual(searchResponseObject.LastName, searchResponseObjectResult.LastName);
    }

}
