using System;
using Amazon.DynamoDBv2.Model;
using FluentAssertions;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;
using ServiceStack.Redis;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase;

namespace SingleViewApi.Tests.V1.UseCase;
[TestFixture]
public class StoreJigsawCredentialsUseCaseTests: LogCallAspectFixture
{

    private Mock<IRedisClient> _mockRedisClient;
    private Mock<IJigsawGateway> _jigsawGatewayMock;
    private StoreJigsawCredentialsUseCase _classUnderTest;

    [SetUp]
    public void Setup()
    {
        _mockRedisClient = new Mock<IRedisClient>();
        _jigsawGatewayMock = new Mock<IJigsawGateway>();
        _classUnderTest = new StoreJigsawCredentialsUseCase(new RedisGateway(_mockRedisClient.Object), new Mock<IJigsawGateway>().Object);

    }

    [Test]
    public void AddsValueAndReturnsId()
    {
        const string value = "testString";
        const int ttl = 1;

        _mockRedisClient.Setup(x => x.SetValue(It.IsAny<string>(), value, TimeSpan.FromDays(ttl)));

        var response = _classUnderTest.Execute(value);

        response.Should().BeOfType<string>();
    }

}
