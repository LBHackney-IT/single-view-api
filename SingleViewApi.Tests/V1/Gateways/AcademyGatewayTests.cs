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
    private string _apiKey;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _mockHttp = new MockHttpMessageHandler();
        _baseUrl = "https://academy.api";
        _apiKey = "mock-api-key";

        var mockClient = _mockHttp.ToHttpClient();
        _classUnderTest = new AcademyGateway(mockClient, _baseUrl, _apiKey);
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
    public void GetCouncilTaxAccountByAccountRefMakesRequestToCouncilTax()
    {
        var accountRef = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"{_baseUrl}/council-tax/{accountRef}")
            .WithHeaders("Authorization", userToken);

        _ = _classUnderTest.GetCouncilTaxAccountByAccountRef(accountRef, userToken);

        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Test]
    public async Task GetCouncilTaxAccountByAccountRefReturnsCouncilTaxRecordResponseObject()
    {
        var accountRef = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedResponse = _fixture.Create<CouncilTaxRecordResponseObject>();

        _mockHttp.Expect($"{_baseUrl}/council-tax/{accountRef}")
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
    public async Task GetHousingBenefitsAccountsByIdReturnsHousingBenefitsRecordResponseObject()
    {
        var accountRef = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var stubbedJson = @"{
          ""claimId"": ""ABC123"",
          ""checkDigit"": ""D"",
          ""personReference"": ""PER0"",
          ""title"": ""Miss"",
          ""firstName"": ""Luna"",
          ""lastName"": ""Kitty"",
          ""dateOfBirth"": ""2021-01-07T14:32:34.511Z"",
          ""fullAddress"": {
            ""line1"": ""123 Cute Street"",
            ""line2"": """",
            ""line3"": ""London"",
            ""line4"": ""string"",
            ""postcode"": ""M3 0W""
          },
          ""postCode"": ""M3 0W"",
          ""householdMembers"": [
            {
              ""title"": ""Mr"",
              ""firstName"": ""Felis"",
              ""lastName"": ""Catus"",
              ""dateOfBirth"": ""2020-03-12T14:32:34.511Z""
            }
          ],
          ""benefits"": [
            {
              ""amount"": 12.30,
              ""description"": ""Cat treats"",
              ""period"": ""2020-2022"",
              ""frequency"": ""Weekly""
            }
          ]
        }";

        _mockHttp.Expect($"{_baseUrl}/benefits/{accountRef}")
            .WithHeaders("Authorization", userToken)
            .Respond("application/json", stubbedJson);

        var results = await _classUnderTest.GetHousingBenefitsAccountByAccountRef(accountRef, userToken);

        Assert.AreEqual("ABC123", results.ClaimId);
        Assert.AreEqual("Luna", results.FirstName);
        Assert.AreEqual("Kitty", results.LastName);
        if (results?.FullAddress != null)
        {
            Assert.AreEqual("123 Cute Street", results.FullAddress.Line1);
            Assert.AreEqual("London", results.FullAddress.Line3);
            Assert.AreEqual("M3 0W", results.FullAddress.Postcode);
        }
        Assert.AreEqual("Felis", results.HouseholdMembers[0].FirstName);
        Assert.AreEqual("Catus", results.HouseholdMembers[0].LastName);
        Assert.AreEqual(12.30, results.Benefits[0].Amount);
        Assert.AreEqual("Weekly", results.Benefits[0].Frequency);
    }

    private static void AreEqualByJson(object expected, object actual)
    {
        var expectedJson = JSON.stringify(expected);
        var actualJson = JSON.stringify(actual);
        Assert.AreEqual(expectedJson, actualJson);
    }
}
