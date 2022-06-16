using AutoFixture;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways;



[TestFixture]
public class EqualityInformationGatewayTests
{
    private readonly Fixture _fixture = new Fixture();
    private EqualityInformationGateway _classUnderTest;
    private MockHttpMessageHandler _mockHttp;

    [SetUp]

    public void Setup()
    {

        _mockHttp = new MockHttpMessageHandler();
        const string baseUrl = "https://equality.api";
        var mockClient = _mockHttp.ToHttpClient();
        _classUnderTest = new EqualityInformationGateway(mockClient, baseUrl);
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
}

