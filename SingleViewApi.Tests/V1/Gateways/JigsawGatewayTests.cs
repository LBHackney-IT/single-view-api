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
    private string _customerBaseUrl;
    private string _homelessnessBaseUrl;
    private string _accommodationBaseUrl;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _authUrl = "https://api.jigsaw-auth";
        _customerBaseUrl = "https://api.jigsaw-customer";
        _homelessnessBaseUrl = "https://api.jigsaw-homelessness";
        _accommodationBaseUrl = "https://api.jigsaw-accommodation";
        _mockHttp = new MockHttpMessageHandler();
        var mockClient = _mockHttp.ToHttpClient();
        _classUnderTest = new JigsawGateway(mockClient, _authUrl, _customerBaseUrl, _homelessnessBaseUrl, _accommodationBaseUrl);
    }

    [Test]
    public void ARequestIsMadeToTheSearchEndpoint()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_customerBaseUrl}/customerSearch?search={firstName}%20{lastName}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}");

        _ = _classUnderTest.GetCustomers(firstName, lastName, bearerToken);

        _mockHttp.VerifyNoOutstandingExpectation();


    }
    [Test]
    public async Task GetCustomersReturnsNullIfThereAreNoResults()
    {

        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_customerBaseUrl}/customerSearch?search={firstName}%20{lastName}")
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

        _mockHttp.Expect($"{_customerBaseUrl}/customerSearch?search={firstName}%20{lastName}")
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

        _mockHttp.Expect($"{_customerBaseUrl}/customerSearch?search={firstName}%20{lastName}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.ServiceUnavailable);

        var searchResults = await _classUnderTest.GetCustomers(firstName, lastName, bearerToken);

        searchResults.Should().BeNull();
    }

    [Test]
    public async Task DataFromSearchApiIsReturned()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();
        var searchResultsResponseObject = _fixture.CreateMany<JigsawCustomerSearchApiResponseObject>().ToList();

        _mockHttp.Expect($"{_customerBaseUrl}/customerSearch?search={firstName}%20{lastName}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond("application/json", searchResultsResponseObject.ToJson());

        var searchResults = await _classUnderTest.GetCustomers(firstName, lastName, bearerToken);

        var searchResponseObject = searchResultsResponseObject[^1];
        var searchResponseObjectResult = searchResults[^1];

        Assert.AreEqual(searchResponseObject.Id, searchResponseObjectResult.Id);
        Assert.AreEqual(searchResponseObject.FirstName, searchResponseObjectResult.FirstName);
        Assert.AreEqual(searchResponseObject.LastName, searchResponseObjectResult.LastName);
    }

    [Test]
    public async Task GetCustomerByIdReturnsNullIfUnauthorised()
    {
        var id = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_customerBaseUrl}/customerOverview/{id}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.Unauthorized);

        var searchResults = await _classUnderTest.GetCustomerById(id, bearerToken);

        searchResults.Should().BeNull();
    }

    [Test]
    public async Task GetCustomerByIdReturnsNullIfApiIsUnavailable()
    {
        var id = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_customerBaseUrl}/customerOverview/{id}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.ServiceUnavailable);

        var searchResults = await _classUnderTest.GetCustomerById(id, bearerToken);

        searchResults.Should().BeNull();
    }

    [Test]
    public async Task DataFromCustomerByIdApiIsReturned()
    {
        var id = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();
        var jigsawCustomerResponseObject = _fixture.Create<JigsawCustomerResponseObject>();

        _mockHttp.Expect($"{_customerBaseUrl}/customerOverview/{id}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond("application/json", jigsawCustomerResponseObject.ToJson());

        var customer = await _classUnderTest.GetCustomerById(id, bearerToken);

        Assert.AreEqual(jigsawCustomerResponseObject.Id, customer.Id);
        Assert.AreEqual(jigsawCustomerResponseObject.PersonInfo.FirstName, customer.PersonInfo.FirstName);
        Assert.AreEqual(jigsawCustomerResponseObject.PersonInfo.LastName, customer.PersonInfo.LastName);
    }

    [Test]
    public async Task GetCustomerNotesByCustomerIdReturnsNullIfUnauthorised()
    {
        var id = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_customerBaseUrl}/customer/{id}/notes")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.Unauthorized);

        var searchResults = await _classUnderTest.GetCustomerNotesByCustomerId(id, bearerToken);

        searchResults.Should().BeNull();
    }

    [Test]
    public async Task GetCustomerNotesByCustomerIdReturnsNullIfApiIsUnavailable()
    {
        var id = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_customerBaseUrl}/customer/{id}/notes")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.ServiceUnavailable);

        var searchResults = await _classUnderTest.GetCustomerNotesByCustomerId(id, bearerToken);

        searchResults.Should().BeNull();
    }

    [Test]
    public async Task DataFromCustomerNotesByCustomerIdApiIsReturned()
    {
        var id = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();
        var jigsawNotesResponseObject = _fixture.CreateMany<JigsawNotesResponseObject>().ToList();

        _mockHttp.Expect($"{_customerBaseUrl}/customer/{id}/notes")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond("application/json", jigsawNotesResponseObject.ToJson());

        var customerNotes = await _classUnderTest.GetCustomerNotesByCustomerId(id, bearerToken);

        Assert.AreEqual(jigsawNotesResponseObject[0].Id, customerNotes[0].Id);
        Assert.AreEqual(jigsawNotesResponseObject[0].Content, customerNotes[0].Content);
        Assert.AreEqual(jigsawNotesResponseObject[0].CustomerId, customerNotes[0].CustomerId);
    }

    [Test]
    public async Task GetCasesByCustomerIdReturnsNullIfUnauthorised()
    {
        var id = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_homelessnessBaseUrl}/casecheck/{id}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.Unauthorized);

        var cases = await _classUnderTest.GetCasesByCustomerId(id, bearerToken);

        cases.Should().BeNull();
    }

    [Test]
    public async Task GetCasesByCustomerIdReturnsNullIfApiIsUnavailable()
    {
        var id = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_homelessnessBaseUrl}/casecheck/{id}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.ServiceUnavailable);

        var cases = await _classUnderTest.GetCasesByCustomerId(id, bearerToken);

        cases.Should().BeNull();
    }

    [Test]
    public async Task DataFromGetCasesByCustomerIdApiIsReturned()
    {
        var id = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();
        var mockCasesResponseObject = _fixture.Create<JigsawCasesResponseObject>();

        _mockHttp.Expect($"{_homelessnessBaseUrl}/casecheck/{id}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond("application/json", mockCasesResponseObject.ToJson());

        var casesObject = await _classUnderTest.GetCasesByCustomerId(id, bearerToken);

        Assert.AreEqual(mockCasesResponseObject.Cases[0].Id, casesObject.Cases[0].Id);
        Assert.AreEqual(mockCasesResponseObject.Cases[0].StatusName, casesObject.Cases[0].StatusName);
        Assert.AreEqual(mockCasesResponseObject.Cases[0].AssignedTo, casesObject.Cases[0].AssignedTo);
    }

    [Test]
    public async Task GetCaseOverviewByCaseIdReturnsNullIfUnauthorised()
    {
        var caseId = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_homelessnessBaseUrl}/caseoverview/{caseId}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.Unauthorized);

        var caseOverview = await _classUnderTest.GetCaseOverviewByCaseId(caseId, bearerToken);

        caseOverview.Should().BeNull();
    }

    [Test]
    public async Task GetCaseOverviewByCaseIdReturnsNullIfApiIsUnavailable()
    {
        var caseId = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_homelessnessBaseUrl}/caseoverview/{caseId}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.ServiceUnavailable);

        var cases = await _classUnderTest.GetCaseOverviewByCaseId(caseId, bearerToken);

        cases.Should().BeNull();
    }

    [Test]
    public async Task DataFromGetCaseOverviewByCaseIdApiIsReturned()
    {
        var caseId = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();
        var mockCaseOverview = _fixture.Create<JigsawCaseOverviewResponseObject>();

        _mockHttp.Expect($"{_homelessnessBaseUrl}/caseoverview/{caseId}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond("application/json", mockCaseOverview.ToJson());

        var casesOverview = await _classUnderTest.GetCaseOverviewByCaseId(caseId, bearerToken);

        Assert.AreEqual(mockCaseOverview.Id, casesOverview.Id);
        Assert.AreEqual(mockCaseOverview.CurrentDecision, casesOverview.CurrentDecision);
        Assert.AreEqual(mockCaseOverview.HouseholdComposition, casesOverview.HouseholdComposition);
    }

    [Test]
    public async Task GetCaseAccommodationPlacementsByCaseIdReturnsNullIfUnauthorised()
    {
        var caseId = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_accommodationBaseUrl}/CaseAccommodationPlacements?caseId={caseId}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.Unauthorized);

        var casePlacementInfo = await _classUnderTest.GetCaseAccommodationPlacementsByCaseId(caseId, bearerToken);

        casePlacementInfo.Should().BeNull();
    }

    [Test]
    public async Task GetCaseAccommodationPlacementsByCaseIdReturnsNullIfApiIsUnavailable()
    {
        var caseId = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_accommodationBaseUrl}/CaseAccommodationPlacements?caseId={caseId}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond(HttpStatusCode.ServiceUnavailable);

        var casePlacementInfo = await _classUnderTest.GetCaseAccommodationPlacementsByCaseId(caseId, bearerToken);

        casePlacementInfo.Should().BeNull();
    }

    [Test]
    public async Task DataFromGetCaseAccommodationPlacementsByCaseIdApiIsReturned()
    {
        var caseId = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();
        var mockCasePlacementInfo = _fixture.Create<JigsawCasePlacementInformationResponseObject>();

        _mockHttp.Expect($"{_accommodationBaseUrl}/CaseAccommodationPlacement?caseId={caseId}")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond("application/json", mockCasePlacementInfo.ToJson());

        var casePlacementInfo = await _classUnderTest.GetCaseAccommodationPlacementsByCaseId(caseId, bearerToken);

        Assert.AreEqual(mockCasePlacementInfo.IsCurrentlyInPlacement, casePlacementInfo.IsCurrentlyInPlacement);
        Assert.AreEqual(mockCasePlacementInfo.Placements[0].Address, casePlacementInfo.Placements[0].Address);
        Assert.AreEqual(mockCasePlacementInfo.Placements[0].Usage, casePlacementInfo.Placements[0].Usage);
    }


    [Test]
    public async Task DataFromGetCaseAdditionalFactorsByCaseIdApiIsReturned()
    {
        var caseId = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();
        var mockData = _fixture.Create<JigsawCaseAdditionalFactorsResponseObject>();

        _mockHttp.Expect($"{_accommodationBaseUrl}/caseform?caseId={caseId}&formId=1&pageId=2")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond("application/json", mockData.ToJson());

        var result = await _classUnderTest.GetCaseAdditionalFactors(caseId, bearerToken);

        Assert.AreEqual(mockData.Title, result.Title);
        Assert.AreEqual(mockData.Header, result.Header);
        Assert.AreEqual(mockData.Footer, result.Footer);
        Assert.AreEqual(mockData.FormTitle, result.FormTitle);
        Assert.AreEqual(mockData.IsBuddyForm, result.IsBuddyForm);
        Assert.AreEqual(mockData.MapCode, result.MapCode);
    }

    [Test]
    public async Task DataFromCaseHealthAndWellBeingByCaseIdApiIsReturned()
    {
        var caseId = _fixture.Create<string>();
        var bearerToken = _fixture.Create<string>();
        var mockData = _fixture.Create<JigsawCaseAdditionalFactorsResponseObject>();

        _mockHttp.Expect($"{_accommodationBaseUrl}/caseform?caseId={caseId}&formId=1&pageId=2")
            .WithHeaders("Authorization", $"Bearer {bearerToken}")
            .Respond("application/json", mockData.ToJson());

        var result = await _classUnderTest.GetCaseAdditionalFactors(caseId, bearerToken);

        Assert.AreEqual(mockData.Title, result.Title);
        Assert.AreEqual(mockData.Header, result.Header);
        Assert.AreEqual(mockData.Footer, result.Footer);
        Assert.AreEqual(mockData.FormTitle, result.FormTitle);
        Assert.AreEqual(mockData.IsBuddyForm, result.IsBuddyForm);
        Assert.AreEqual(mockData.MapCode, result.MapCode);
    }
}
