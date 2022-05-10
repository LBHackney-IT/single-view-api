using System;
using AutoFixture;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class JigsawGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private JigsawGateway _classUnderTest;
        private MockHttpMessageHandler _mockHttp;

        [SetUp]
        public void Setup()
        {
            _mockHttp = new MockHttpMessageHandler();
            const string baseUrl = "http://jigsaw.api";
            var mockClient = _mockHttp.ToHttpClient();
            _classUnderTest = new JigsawGateway(mockClient, baseUrl);
        }

        [Test]

        public void ARequestIsMade()
        {
            const string username = "testUser@test.com";

            _mockHttp.Expect($"http://jigsaw.api/authorise/");

            _ = _classUnderTest.GetAuthToken(username);

            _mockHttp.VerifyNoOutstandingExpectation();
        }


    }
}
