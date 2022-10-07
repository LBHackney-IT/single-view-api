using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways;

[TestFixture]
public class ContactDetailsGatewayTest
{
    [SetUp]
    public void Setup()
    {
        _mockHttp = new MockHttpMessageHandler();
        const string baseUrl = "https://contact-details.api";
        var mockClient = _mockHttp.ToHttpClient();
        _classUnderTest = new ContactDetailsGateway(mockClient, baseUrl);
    }

    private readonly Fixture _fixture = new();
    private ContactDetailsGateway _classUnderTest;
    private MockHttpMessageHandler _mockHttp;

    [Test]
    public void ARequestIsMade()
    {
        // Arrange
        const string id = "123-Some-ID";
        const string userToken = "User token";

        _mockHttp.Expect($"https://contact-details.api/contactDetails?targetId={id}&includeHistoric=true")
            .WithHeaders("Authorization", userToken);
        // Act
        _ = _classUnderTest.GetContactDetailsById(id, userToken);

        // Assert
        _mockHttp.VerifyNoOutstandingExpectation();
    }


    [Test]
    public async Task GetContactDetailsByIdReturnsNullIfEntityDoesntExist()
    {
        // Arrange
        const string id = "123-Some-ID";
        const string userToken = "User token";

        _mockHttp.Expect($"https://contact-details.api/contactDetails?targetId={id}&includeHistoric=true")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.NotFound, x => new StringContent(id));

        var contactDetails = await _classUnderTest.GetContactDetailsById(id, userToken);

        contactDetails.Should().BeEmpty();
    }

    [Test]
    public async Task GetContactDetailsByIdReturnsNullIfUserIsUnAuthorised()
    {
        // Arrange
        const string id = "123-Some-ID";
        const string userToken = "User token";

        _mockHttp.Expect($"https://contact-details.api/contactDetails?targetId={id}&includeHistoric=true")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.Unauthorized, x => new StringContent(id));

        var contactDetails = await _classUnderTest.GetContactDetailsById(id, userToken);

        contactDetails.Should().BeEmpty();
    }

    [Test]
    public async Task GetContactDetailsByIdReturnsNullIfApiIsDown()
    {
        // Arrange
        const string id = "123-Some-ID";
        const string userToken = "User token";

        _mockHttp.Expect($"https://contact-details.api/contactDetails?targetId={id}&includeHistoric=true")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.ServiceUnavailable, x => new StringContent(id));

        var contactDetails = await _classUnderTest.GetContactDetailsById(id, userToken);

        contactDetails.Should().BeEmpty();
    }

    [Test]
    public async Task DataFromApiIsReturned()
    {
        // Arrange
        const string id = "123-Some-ID";
        const string userToken = "User token";

        var rawJson = @"
{
    ""results"": [
        {
            ""id"": ""eafeaa0f-6e2c-7c4e-719f-275e02a5829a"",
            ""targetId"": ""6899ab97-d3fa-a04b-fde7-2c8a4ef523f5"",
            ""targetType"": ""person"",
            ""contactInformation"": {
                ""contactType"": ""address"",
                ""subType"": ""correspondenceAddress"",
                ""value"": ""1 Hillman st, E8 1DY"",
                ""description"": """",
                ""addressExtended"": {
                    ""uprn"": ""100021038870"",
                    ""isOverseasAddress"": false,
                    ""overseasAddress"": null,
                    ""addressLine1"": ""1 Hillman st"",
                    ""addressLine2"": """",
                    ""addressLine3"": """",
                    ""addressLine4"": """",
                    ""postCode"": ""E8 1DY""
                }
            },
            ""sourceServiceArea"": {
                ""area"": ""Housing"",
                ""isDefault"": true
            },
            ""recordValidUntil"": null,
            ""isActive"": true,
            ""createdBy"": {
                ""createdAt"": ""2021-10-23T15:56:31.303552"",
                ""fullName"": ""Import"",
                ""emailAddress"": """"
            }
        },
        {
            ""id"": ""587900ca-ea32-4985-8bc7-ea0a416848c6"",
            ""targetId"": ""6899ab97-d3fa-a04b-fde7-2c8a4ef523f5"",
            ""targetType"": ""person"",
            ""contactInformation"": {
                ""contactType"": ""phone"",
                ""subType"": ""home"",
                ""value"": ""02083563154"",
                ""description"": null,
                ""addressExtended"": null
            },
            ""sourceServiceArea"": {
                ""area"": ""Housing"",
                ""isDefault"": true
            },
            ""recordValidUntil"": null,
            ""isActive"": true,
            ""createdBy"": {
                ""createdAt"": ""2022-07-21T15:35:37.7224949Z"",
                ""fullName"": ""Testy McTest"",
                ""emailAddress"": ""testy.mctest@hackney.gov.uk""
            }
        },
        {
            ""id"": ""ce44c6e0-b78d-4884-853a-c1a71e757d18"",
            ""targetId"": ""6899ab97-d3fa-a04b-fde7-2c8a4ef523f5"",
            ""targetType"": ""person"",
            ""contactInformation"": {
                ""contactType"": ""phone"",
                ""subType"": ""other"",
                ""value"": ""02083563100"",
                ""description"": ""Rent phone"",
                ""addressExtended"": null
            },
            ""sourceServiceArea"": {
                ""area"": ""Housing"",
                ""isDefault"": true
            },
            ""recordValidUntil"": null,
            ""isActive"": true,
            ""createdBy"": {
                ""createdAt"": ""2022-07-21T15:36:17.2296532Z"",
                ""fullName"": ""Testy McTest"",
                ""emailAddress"": ""testy.mctest@hackney.gov.uk""
            }
        }
    ]
}
";

        _mockHttp.Expect($"https://contact-details.api/contactDetails?targetId={id}&includeHistoric=true")
            .WithHeaders("Authorization", userToken)
            .Respond("application/json", rawJson);

        // Act
        var contactDetails = await _classUnderTest.GetContactDetailsById(id, userToken);

        // Assert
        _mockHttp.VerifyNoOutstandingExpectation();
        Assert.AreEqual("address", contactDetails[0].ContactInformation.ContactType.ToString());
        Assert.AreEqual("correspondenceAddress", contactDetails[0].ContactInformation.SubType.ToString());
        Assert.AreEqual("100021038870", contactDetails[0].ContactInformation.AddressExtended.UPRN);
        Assert.AreEqual(false, contactDetails[0].ContactInformation.AddressExtended.IsOverseasAddress);
        Assert.AreEqual(null, contactDetails[0].ContactInformation.AddressExtended.OverseasAddress);
        Assert.AreEqual("1 Hillman st, E8 1DY", contactDetails[0].ContactInformation.Value);
        Assert.AreEqual(true, contactDetails[0].IsActive);
        Assert.AreEqual("Housing", contactDetails[0].SourceServiceArea.Area);
        Assert.AreEqual("phone", contactDetails[1].ContactInformation.ContactType.ToString());
        Assert.AreEqual("home", contactDetails[1].ContactInformation.SubType.ToString());
        Assert.AreEqual("02083563154", contactDetails[1].ContactInformation.Value);
        Assert.AreEqual(true, contactDetails[1].IsActive);
        Assert.AreEqual("Housing", contactDetails[1].SourceServiceArea.Area);
        Assert.AreEqual("phone", contactDetails[2].ContactInformation.ContactType.ToString());
        Assert.AreEqual("other", contactDetails[2].ContactInformation.SubType.ToString());
        Assert.AreEqual("02083563100", contactDetails[2].ContactInformation.Value);
        Assert.AreEqual("Rent phone", contactDetails[2].ContactInformation.Description);
        Assert.AreEqual(true, contactDetails[2].IsActive);
        Assert.AreEqual("Housing", contactDetails[2].SourceServiceArea.Area);
    }
}
