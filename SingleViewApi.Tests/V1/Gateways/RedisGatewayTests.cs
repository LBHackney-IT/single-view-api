using System;
using AutoFixture;
using SingleViewApi.V1.Gateways;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using ServiceStack.Redis;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class RedisGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private RedisGateway _classUnderTest;
        private Mock<IRedisClient> _mockRedisClient;

        [SetUp]
        public void Setup()
        {
            _mockRedisClient = new Mock<IRedisClient>();
            _classUnderTest = new RedisGateway(_mockRedisClient.Object);
        }

        [Test]
        public void AddsValueWithTtl()
        {
            const string value = "testyString";
            const int ttl = 3;

            _mockRedisClient.Setup(x => x.SetValue(It.IsAny<string>(), value, TimeSpan.FromDays(ttl)));

            var response = _classUnderTest.AddValue(value, ttl);

            response.Should().BeOfType<string>();
        }

        [Test]
        public void GetsValue()
        {
            const string id = "5019c056-2936-4c51-a8f4-dfa6584d0754";
            var value = _fixture.Create<string>();

            _mockRedisClient.Setup(x => x.Get<string>(id)).Returns(value);

            var response = _classUnderTest.GetValue(id);

            response.Should().BeEquivalentTo(value);

        }


    }
}
