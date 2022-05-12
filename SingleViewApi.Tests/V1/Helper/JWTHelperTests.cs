using FluentAssertions;
using NUnit.Framework;
using SingleViewApi.V1.Helpers;


namespace SingleViewApi.Tests.V1.Helper
{

    public class JwtHelperTests
    {
        private JwtHelper _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new JwtHelper();

        }

        [Test]
        public void ItDecodesAJwt()
        {
            const string jwt =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6ImEgY2hpbGQifQ.eyJwYXNzd29yZCI6IjEyMzQ1Njc4OTAiLCJ1c2VybmFtZSI6IkpvaG4gRG9lIn0.8zMil45BiIrXfDyvADeaKDJwmbLvFYGDZShplUd_IgU";
            const string expectedUsername = "test";

            var result = _classUnderTest.DecodeJigsawCredentials(jwt);

            result.Username.Should().Be(expectedUsername);
        }
    }
}
