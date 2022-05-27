using System;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Redis;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.Helpers;
using SingleViewApi.V1.Helpers.Interfaces;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase;

[TestFixture]
public class GetJigsawAuthTokenUseCaseTests : LogCallAspectFixture
{
    private Mock<IRedisGateway> _mockRedisGateway;
    private Mock<IJigsawGateway> _jigsawGatewayMock;
    private Mock<IDecoderHelper> _decoderHelperMock;
    private GetJigsawAuthTokenUseCase _classUnderTest;

    [SetUp]
    public void Setup()
    {
        _mockRedisGateway = new Mock<IRedisGateway>();
        _jigsawGatewayMock = new Mock<IJigsawGateway>();
        _decoderHelperMock = new Mock<IDecoderHelper>();
        _classUnderTest = new GetJigsawAuthTokenUseCase(_jigsawGatewayMock.Object, _mockRedisGateway.Object, _decoderHelperMock.Object);
    }

    [Test]
    public void ReturnsAuthTokenCachedWhenMatchingHackneyTokenIsProvided()
    {
        const string hackneyToken = "hackneyToken";
        const string redisId = "redisId";
        const string jigsawToken = "jigsawToken";

        _mockRedisGateway.Setup(x => x.GetValue(hackneyToken)).Returns(jigsawToken);

        var result = _classUnderTest.Execute(redisId, hackneyToken).Result;

        Assert.That(result.Token, Is.EqualTo(jigsawToken));
    }
    [Test]
    public void UsesRedisIdToFetchCredentialsWhenNoMatchingHackneyTokenProvided()
    {
        const string hackneyToken = "hackneyToken";
        const string mockJigsawUsername = "mockJigsawUsername";
        const string mockJigsawPassword = "mockJigsawPassword";
        const string redisId = "redisId";
        const string jigsawToken = "jigsawToken";
        const string encryptedCredentials = "encryptedCredentials";
        JigsawCredentials mockJigsawCredentials = new JigsawCredentials
        {
            Username = mockJigsawUsername,
            Password = mockJigsawPassword
        };
        AuthGatewayResponse mockAuthGatewayResponse = new AuthGatewayResponse
        {
            Token = jigsawToken
        };

        _mockRedisGateway.Setup(x => x.GetValue(hackneyToken)).Returns("");
        _mockRedisGateway.Setup(x => x.GetValue(redisId)).Returns(encryptedCredentials);

        _decoderHelperMock.Setup(x => x.DecodeJigsawCredentials(encryptedCredentials)).Returns(mockJigsawCredentials);
        _jigsawGatewayMock.Setup(x => x.GetAuthToken(mockJigsawCredentials)).Returns(mockAuthGatewayResponse.AsTaskResult());
        _mockRedisGateway.Setup(x => x.AddValueWithKey(hackneyToken, mockAuthGatewayResponse.Token, 1));

        var result = _classUnderTest.Execute(redisId, hackneyToken).Result;

        Assert.That(result.Token, Is.EqualTo(jigsawToken));
        _jigsawGatewayMock.Verify(x => x.GetAuthToken(mockJigsawCredentials), Times.Once);
        _mockRedisGateway.Verify(x => x.AddValueWithKey(hackneyToken, jigsawToken, 1), Times.Once);

    }

}
