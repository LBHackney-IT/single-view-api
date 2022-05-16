using FluentAssertions;
using NUnit.Framework;
using SingleViewApi.V1.Helpers;


namespace SingleViewApi.Tests.V1.Helper
{

    public class JwtHelperTests
    {
        private DecoderHelper _classUnderTest;

        [SetUp]
        public void Setup()
        {
            const string testKey = "GVitnm.QjsXUYjTTJ@_@.hAr-Lh2GVAX";
            const string testiv = "J@_@.hAr-Lh2GVAX";
            _classUnderTest = new DecoderHelper(testKey, testiv);

          }

        [Test]
        public void ItDecodesTheEncryptedCredentials()
        {
            const string encyptedCredentials =
                "9JXbsm6zx5QFg7eUKIfxk8A+ZBGn+snIWDmSjLQNBUYVquE71prAbMhn4IoGqqoDz9zDjs7To547TjXdzajBsA==";
            const string expectedUsername = "testUser";

            var result = _classUnderTest.DecodeJigsawCredentials(encyptedCredentials);

            result.Username.Should().Be(expectedUsername);
        }
    }
}
