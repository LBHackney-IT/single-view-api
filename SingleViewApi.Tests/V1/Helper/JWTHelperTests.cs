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
                "NlPY0psUQks028XLpqHJppmAYBS17gk30ozlCCdR/3UFpuI9QfwndbmikmI3xAUl0UQ=";
            const string expectedUsername = "test";

            var result = _classUnderTest.DecodeJigsawCredentials(jwt);

            result.Username.Should().Be(expectedUsername);
        }
    }
}
