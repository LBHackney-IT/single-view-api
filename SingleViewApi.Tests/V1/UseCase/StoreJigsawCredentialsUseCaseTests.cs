using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using ServiceStack.Redis;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.Helpers;
using SingleViewApi.V1.Helpers.Interfaces;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase;
[TestFixture]
public class StoreJigsawCredentialsUseCaseTests : LogCallAspectFixture
{

    private Mock<IRedisClient> _mockRedisClient;
    private Mock<IJigsawGateway> _jigsawGatewayMock;
    private Mock<IDecoderHelper> _decoderHelperMock;
    private StoreJigsawCredentialsUseCase _classUnderTest;

    [SetUp]
    public void Setup()
    {
        _mockRedisClient = new Mock<IRedisClient>();
        _jigsawGatewayMock = new Mock<IJigsawGateway>();
        _decoderHelperMock = new Mock<IDecoderHelper>();
        _classUnderTest = new StoreJigsawCredentialsUseCase(new RedisGateway(_mockRedisClient.Object), _jigsawGatewayMock.Object, _decoderHelperMock.Object);

    }

    [Test]
    public void AddsValueAndReturnsId()
    {
        const string encryptedCreds = "testString";
        const string id = "testId";
        const int ttl = 1;
        var mockJigsawCredentials = new JigsawCredentials()
        {
            Username = "TestUsername",
            Password = "TestPassword",
        };

        _decoderHelperMock.Setup(x => x.DecodeJigsawCredentials(encryptedCreds)).Returns(mockJigsawCredentials);
        _jigsawGatewayMock.Setup(x => x.GetAuthToken(mockJigsawCredentials)).ReturnsAsync(new AuthGatewayResponse() { Token = id });
        _mockRedisClient.Setup(x => x.SetValue(It.IsAny<string>(), id, TimeSpan.FromDays(ttl)));

        var response = _classUnderTest.Execute(encryptedCreds);

        response.Should().BeOfType<string>();
    }

    [Test]
    public void ReturnsNullIfCredentialsAreIncorrect()
    {
        const string value = "testString";
        const JigsawCredentials mockJigsawCredentials = null;
        _decoderHelperMock.Setup(x => x.DecodeJigsawCredentials(value)).Returns(mockJigsawCredentials);
        _jigsawGatewayMock.Setup(x => x.GetAuthToken(mockJigsawCredentials))
            .ReturnsAsync(new AuthGatewayResponse() { Token = null, ExceptionMessage = "Credentials Are Incorrect" }); ;

        var response = _classUnderTest.Execute(value);

        response.Should().BeNull();
    }

}
