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
            //             const string testKey = @"-----BEGIN RSA PRIVATE KEY-----
            // MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAK2xcApwIKTaK3S/KNaU3xBKsZvHq211GKAf0Q7Rr2ZslTZk0r3oaAaCeEWrTamkqj7BmqcZle0KHoTQAwCe+op3oJL4iO969fplvvm7IZnfLAJlu6Na96xvpA5u4EwuxzZC6ypfCL0rO90F0apHu3X00xWpqA9M2Cv1FfMARuflAgMBAAECgYAlDHMElvQvlaWtSlUQqEKIPBZ0Tvu+5joxdXfnPEy4gTK2nnzhKyB9Ovkiiy6On+P8bNlrCScUn5Lw61momUCBTIMcOqoWCnIa/LzrtzrmTIWoK9CbDXWb3BtHpWbrOA5ImthjUnhuiC8Se4TBHysBKP16rKYYcOFghrza6RzHBQJBANzgrlZYsYVue3IHphn6Z4Z2wtWTnG3YOYXTbKIZJ3bprTUxMqCdCCeOSlin/5XrhXuvJb6JrYtQ/fWnNKPEZbsCQQDJUAEYq0HQPa+TjiZ8GCIvwrIfKwhoj1Yqtt6yc5ZaHGkmUjhyaD7wIHUpvjYrUWOlGT+YxLy3zpypiWn+PT7fAkBuo60wW2Uw7VKwn9w4DxJkbIDT8wjcjP7jZjB4UhlOm2bDyk0N3xsClWfwWNbSBhPrDKTfeJ+RtZRAxOy9S9eZAkBeA3zflmr/4y7xy/rxDRw6DcS9hY1Bt17YR9qsoAphPs9kVBWEaGasIZxVyDzn8fSFD8uBfq9bb6P+EOxtDRElAkEAtTc4kV/DtIDYEWjSTcOK+HuAitvGpl4/2nwtE1tpHGKzRZNAgSdsCL4oJ4TGD7mBbMGhRf2SlpQ5dBHF3MFQOg==
            // -----END RSA PRIVATE KEY-----";
            const string testKey = @"MIIEpAIBAAKCAQEAz1xzhzn9ulK+rQ0gOCb/xzquQgelF8BGtyXRiMqs0hLbwDSG
smNm27l7+ZJ4FyUEWz7WBQdpXg7GKgqU4JOo5z/rS5L2CtYxMv09G0JL28GSeCKb
mKyc0FU7XXuFXT52E6MAHAEX8A8It7PkQxnYn7GF0T5BOzrTE/lJGvrZ9SNbvTho
lxthdy7EyNSoTGwqplc64xLK/puEWnDqBeXscPBKUnqkTCtjCi8v16f+u5qQ3Pvo
6IWq8BhPLP0OttTG1dGm8hT0V8oRsXs1ueU6bdGySzcZhHHraidNjR0DD0cCGj5w
tJv6uGJt5IjTwtgkwqXzC1PjgbreLSnmESqxbwIDAQABAoIBAGoDmEUQjvoNh9Kt
nUVYGvjetWyov+0OrpjASwompIiaf6Mat6rISzQW9p04FxGuKTYFQCI+Ec62uvWP
h2Hx3C6ubTusHQKZU3oAIpenI2Vs59dnyikaRun0SW+X0S1E29VaRpoKrRRce1aA
MDlmI2wxtHqv22XOw0+46XlTus8qowRVUpiq0VIhdTS2MHp72JW8A7i2mDXJD6T2
5xroJRP4sjqgowxgfzm7qLWKVRzhd8t1AWsy5/as7tsHbYPnePAk+Kf7/EUAI234
YkRJExOzv84YatcLWnxgmfSYlKmGPkQHM1MEyCfm944HQE5Kz4oL58kixMzj4lxT
qHKuQSECgYEA6fk/914LqqdPMTvfMD9XAI+988003Np4ayd0NBIUCB7aYy59Ydzr
A3GSEH9Zs46SlEog6hLUE4MnndsK8sbxcQaeKCBvvmy2bkTiay+FpiOox2aYdk/b
Jro0/S28S3ewBDHnNZTiDhveutG5MckwCH//mTE3H5buIUXJjSZYGJkCgYEA4uHX
iRd2maI1TpY9LEDCkDK1/ppRoTboVSk38FOi8wEww9y+Tg44uIN9Jsr3IHD+9zvL
W0BxdtWu/RUzxEqOmLJSMEC3h8kWyG7/UubT1z6SFYcdTv8uT+Hv6Y3MAnetLmRB
17xS1Kk8HOaolm0n84Pbf8F9naQqnSYK/3XrN0cCgYEAuot7sKJO8vUKctqljDY0
C8KPVH7cEuos8GI4h/uR9ReET2eVmB0nU0uNhZHD1yFpRMoFjBsO86+yOm2WQRLV
FaSNHLkf3teWbDyHuaXeDSSJQNJP5KSuuiXkcaexZQo8UFvpWBMoWM9nudPUsBkU
LvK+u0k+BfW0TtrxY5Sa0KECgYEAhwuIgIhfxsi/VBynIs0VQkF9BKQUTJJjLjWP
n1QZHO2rehnNiKZ8ao+RkeKjwEKh8MU9oP41y1cbhnb1TjRWtlsGAsSyuXcfQYSY
I19T3r3WtEutTVUk2tWQOQN22E0l3wQ8EM5+uyhUqJs+4/LrIc5te5jPTWLnHo0D
ZNVqgYcCgYAk68DGImt0zqHSbJ/eOe0bIfBeDkt2t1qC9EcnXgIu6/5J8ccKDDh+
XJfBDhQmsAyVRawVNyEiGKQgiTpGPtdy7Cn/emuMiE36jnPZ6T4tanqSiPeOkrsq
+0Jcqzg1nlf8KGjZh1MYlRRG2OPX+9Za/eJTimuuKs85kW7sAzMrGA==";
            _classUnderTest = new DecoderHelper(testKey);

        }

        [Test]
        public void ItDecodesTheEncryptedCredentials()
        {
            const string encryptedCredentials = "e9AfdDArc1+Bw/D9eBAsT4WBrUc3VSjnUIgdDa+NrwFjwIYUyupVfxDXjy8Ju0LPNlIcxDlmL6AvC2PlIcJ1h0WvAiaX9SG4C7mu+mYeCb/bJjJKIOScDCaNrvdRiFcwDs3azjXB2S4N5efPRauv7NGaZDxyllVv0sJwNMt9BVPYkvanXnmRIYFm15YSPI7qYB+VmC1xmoLSdpeBQFOhT6B90kGSFK3ZUcc2xhzkNJbpfSHqOeb8jG/xMD6Lk97O1kF0dbZFhfUHwCAcnwNJCYJ0SxqSIc7JGt/Xg0Nx2sD929YDgst6L/nZ/DYuZnqAHmT5Zv8wL1/ZCjlXhb7rxw==";
            const string expectedUsername = "testUser";
            const string expectedPassword = "pa$$w0rd";

            var result = _classUnderTest.DecodeJigsawCredentials(encryptedCredentials);

            result.Username.Should().Be(expectedUsername);
            result.Password.Should().Be(expectedPassword);
        }
    }
}
