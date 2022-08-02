using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Newtonsoft.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways;



[TestFixture]
public class EqualityInformationGatewayTests : LogCallAspectFixture
{
    private readonly Fixture _fixture = new Fixture();
    private EqualityInformationGateway _classUnderTest;
    private MockHttpMessageHandler _mockHttp;
    private HttpClient _mockClient;

    [SetUp]

    public void Setup()
    {

        _mockHttp = new MockHttpMessageHandler();
        const string baseUrl = "https://equality.api";
        _mockClient = _mockHttp.ToHttpClient();
        _classUnderTest = new EqualityInformationGateway(_mockClient, baseUrl);
    }

    [Test]
    public void ARequestIsMade()
    {
        var id = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"https://equality.api/equality-information?targetId={id}")
            .WithHeaders("Authorization", userToken);

        _ = _classUnderTest.GetEqualityInformationById(id, userToken);

        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Test]
    public async Task DataFromApiIsRetrieved()
    {
        // Arrange
        var id = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();
        var data = new EqualityInformationResponseObject
        {
            Gender = new Gender()
            {
                GenderValue = "Female",
                GenderValueIfOther = null,
                GenderDifferentToBirthSex = false
            },
            Ethnicity = new Ethnicity()
            {
                EthnicGroupValue = "Other",
                EthnicGroupValueIfOther = "Other ethnicity"
            },
            ReligionOrBelief = new ReligionOrBelief()
            {
                ReligionOrBeliefValue = "Other",
                ReligionOrBeliefValueIfOther = "Other religion"
            },
            MarriageOrCivilPartnership = new MarriageOrCivilPartnership()
            {
                Married = "No",
                CivilPartnership = "Yes"
            },
            PregnancyOrMaternity = new List<PregnancyOrMaternity>()
            {
                new PregnancyOrMaternity()
                {
                    PregnancyDate = "2022-01-20",
                    PregnancyValidUntil = "2022-09-20"
                }
            },
            NationalInsuranceNumber = "SL203040",
            Disabled = "No",
            CommunicationRequirements = new List<string>()
            {
                "Communication help needed",
                "Interpretation needed"
            }
        };

        _mockHttp.Expect($"https://equality.api/equality-information?targetId={id}")
            .WithHeaders("Authorization", userToken)
            .Respond("application/json", JsonConvert.SerializeObject(data));

        // Act
        var result = await _classUnderTest.GetEqualityInformationById(id, userToken);

        // Assert
        result.Gender?.GenderValue.Should().Be("Female");
        result.Gender?.GenderValueIfOther.Should().BeNull();
        result.Gender?.GenderDifferentToBirthSex.Should().BeFalse();
        result.Ethnicity?.EthnicGroupValue.Should().Be("Other");
        result.Ethnicity?.EthnicGroupValueIfOther.Should().Be("Other ethnicity");
        result.ReligionOrBelief?.ReligionOrBeliefValue.Should().Be("Other");
        result.ReligionOrBelief?.ReligionOrBeliefValueIfOther.Should().Be("Other religion");
        result.MarriageOrCivilPartnership?.Married.Should().Be("No");
        result.MarriageOrCivilPartnership?.CivilPartnership.Should().Be("Yes");
        result.PregnancyOrMaternity?[0].PregnancyDate.Should().Be("2022-01-20");
        result.PregnancyOrMaternity?[0].PregnancyValidUntil.Should().Be("2022-09-20");
        result.NationalInsuranceNumber.Should().Be("SL203040");
        result.Disabled.Should().Be("No");
        result.CommunicationRequirements?.Count.Should().Be(2);
        result.CommunicationRequirements?[1].Should().Be("Interpretation needed");
    }

    [Test]
    public async Task GetEqualityInformationByIdReturnsNullIfApiIsNotResponding()
    {
        // Arrange
        var id = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"https://equality.api/equality-information?targetId={id}")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.ServiceUnavailable, x => x.Content);

        // Act
        var result = await _classUnderTest.GetEqualityInformationById(id, userToken);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetEqualityInformationByIdReturnsNullIfEntityDoesExist()
    {
        // Arrange
        var id = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"https://equality.api/equality-information?targetId={id}")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.NotFound, x => x.Content);

        // Act
        var result = await _classUnderTest.GetEqualityInformationById(id, userToken);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetEqualityInformationByIdReturnsNullIfUserNotAuthorised()
    {
        // Arrange
        var id = _fixture.Create<string>();
        var userToken = _fixture.Create<string>();

        _mockHttp.Expect($"https://equality.api/equality-information?targetId={id}")
            .WithHeaders("Authorization", userToken)
            .Respond(HttpStatusCode.Unauthorized, x => x.Content);

        // Act
        var result = await _classUnderTest.GetEqualityInformationById(id, userToken);

        // Assert
        result.Should().BeNull();
    }
}
