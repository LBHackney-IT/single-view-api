using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using JsonSerializer = System.Text.Json.JsonSerializer;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using ServiceStack;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways;

[TestFixture]
public class AcademyGatewayTests
{
    private Fixture _fixture;
    private AcademyGateway _classUnderTest;
    private MockHttpMessageHandler _mockHttp;
    private string _baseUrl;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _mockHttp = new MockHttpMessageHandler();
        _baseUrl = "https://academy.api";
        var mockClient = _mockHttp.ToHttpClient();
        _classUnderTest = new AcademyGateway(mockClient, _baseUrl);
    }

    [Test]
    public void GetCouncilTaxAccountsByCustomerNameMakesRequestToCouncilTaxSearch()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_baseUrl}/council-tax/search?firstName={firstName}&lastName={lastName}")
            .WithHeaders("Authorization", userToken);

        _ = _classUnderTest.GetCouncilTaxAccountsByCustomerName(firstName, lastName, userToken);

        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Test]
    public async Task GetCouncilTaxAccountsByCustomerNameReturnsNullIfNotFound()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_baseUrl}/council-tax/search?firstName={firstName}&lastName={lastName}")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.NotFound);

        var results = await _classUnderTest.GetCouncilTaxAccountsByCustomerName(firstName, lastName, userToken);

        results.Should().BeNull();
    }

    [Test]
    public async Task GetCouncilTaxAccountsByCustomerNameReturnsCouncilTaxSearchResponseObject()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedResponse = _fixture.Create<CouncilTaxSearchResponseObject>();

        _mockHttp.Expect($"{_baseUrl}/council-tax/search?firstName={firstName}&lastName={lastName}")
            .WithHeaders("Authorization", userToken)
            .Respond("application/json",
                JsonSerializer.Serialize<CouncilTaxSearchResponseObject>(stubbedResponse));

        var results = await _classUnderTest.GetCouncilTaxAccountsByCustomerName(firstName, lastName, userToken);

        Assert.AreEqual(stubbedResponse.Customers[0].Id, results.Customers[0].Id);
        Assert.AreEqual(stubbedResponse.Customers[0].FirstName, results.Customers[0].FirstName);
        Assert.AreEqual(stubbedResponse.Customers[0].LastName, results.Customers[0].LastName);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Line1, results.Customers[0].FullAddress.Line1);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Line2, results.Customers[0].FullAddress.Line2);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Line3, results.Customers[0].FullAddress.Line3);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Postcode, results.Customers[0].FullAddress.Postcode);
    }

    [Test]
    public void GetHousingBenefitsAccountsByCustomerNameMakesRequestToHousingBenefitsSearch()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_baseUrl}/benefits/search?firstName={firstName}&lastName={lastName}")
            .WithHeaders("Authorization", userToken);

        _ = _classUnderTest.GetHousingBenefitsAccountsByCustomerName(firstName, lastName, userToken);

        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Test]
    public async Task GetHousingBenefitsAccountsByCustomerNameReturnsNullIfNotFound()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_baseUrl}/council-tax/search?firstName={firstName}&lastName={lastName}")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.NotFound);

        var results = await _classUnderTest.GetHousingBenefitsAccountsByCustomerName(firstName, lastName, userToken);

        results.Should().BeNull();
    }

    [Test]
    public async Task GetHousingBenefitsAccountsByCustomerNameReturnsHousingBenefitsSearchResponseObject()
    {
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedResponse = _fixture.Create<HousingBenefitsSearchResponseObject>();

        _mockHttp.Expect($"{_baseUrl}/benefits/search?firstName={firstName}&lastName={lastName}")
            .WithHeaders("Authorization", userToken)
            .Respond("application/json",
                JsonSerializer.Serialize<HousingBenefitsSearchResponseObject>(stubbedResponse));

        var results = await _classUnderTest.GetHousingBenefitsAccountsByCustomerName(firstName, lastName, userToken);

        Assert.AreEqual(stubbedResponse.Customers[0].Id, results.Customers[0].Id);
        Assert.AreEqual(stubbedResponse.Customers[0].FirstName, results.Customers[0].FirstName);
        Assert.AreEqual(stubbedResponse.Customers[0].LastName, results.Customers[0].LastName);
        Assert.AreEqual(stubbedResponse.Customers[0].DateOfBirth, results.Customers[0].DateOfBirth);
        Assert.AreEqual(stubbedResponse.Customers[0].NiNumber, results.Customers[0].NiNumber);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Line1, results.Customers[0].FullAddress.Line1);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Line2, results.Customers[0].FullAddress.Line2);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Line3, results.Customers[0].FullAddress.Line3);
        Assert.AreEqual(stubbedResponse.Customers[0].FullAddress.Postcode, results.Customers[0].FullAddress.Postcode);
    }

    [Test]
    public void ARequestIsMadeToGetAccountByAccountRef()
    {
        var accountRef = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"https://academy.api/council-tax/{accountRef}")
            .WithHeaders("Authorization", userToken);

        _ = _classUnderTest.GetCouncilTaxAccountByAccountRef(accountRef, userToken);

        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Test]
    public async Task CouncilTaxRecordResponseObjectWhenGotById()
    {
        var accountRef = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedResponse = _fixture.Create<CouncilTaxRecordResponseObject>();

        _mockHttp.Expect($"https://academy.api/council-tax/{accountRef}")
            .WithHeaders("Authorization", userToken)
            .Respond("application/json",
                JsonSerializer.Serialize<CouncilTaxRecordResponseObject>(stubbedResponse));

        var result = await _classUnderTest.GetCouncilTaxAccountByAccountRef(accountRef, userToken);

        _mockHttp.VerifyNoOutstandingExpectation();

        Assert.AreEqual(stubbedResponse.Title, result.Title);
        Assert.AreEqual(stubbedResponse.AccountBalance, result.AccountBalance);
        Assert.AreEqual(stubbedResponse.AccountReference, result.AccountReference);
        Assert.AreEqual(stubbedResponse.FirstName, result.FirstName);
        AreEqualByJson(stubbedResponse.ForwardingAddress, result.ForwardingAddress);
        Assert.AreEqual(stubbedResponse.LastName, result.LastName);
        AreEqualByJson(stubbedResponse.PropertyAddress, result.PropertyAddress);
        Assert.AreEqual(stubbedResponse.AccountCheckDigit, result.AccountCheckDigit);
    }

    public static void AreEqualByJson(object expected, object actual)
    {
        var expectedJson = JSON.stringify(expected);
        var actualJson = JSON.stringify(actual);
        Assert.AreEqual(expectedJson, actualJson);
    }
}
