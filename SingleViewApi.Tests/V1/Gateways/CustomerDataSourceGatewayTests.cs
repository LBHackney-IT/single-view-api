using System;
using System.Diagnostics;
using System.Linq;
using AutoFixture;
using SingleViewApi.V1.Gateways;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using ServiceStack.Redis;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.Tests.V1.Gateways
{
    [TestFixture]
    public class CustomerDataSourceGatewayTests : DatabaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private CustomerDataSourceGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new CustomerDataSourceGateway(SingleViewContext);
        }

        [Test]
        public void AddsValue()
        {
            var customerDataSource = _fixture.Create<CustomerDataSource>();

            _ = _classUnderTest.Add(customerDataSource);

            var actual = SingleViewContext.CustomerDataSources.ToList().LastOrDefault();

            Assert.AreEqual(customerDataSource.CustomerId, actual.CustomerId);
            Assert.AreEqual(customerDataSource.SourceId, actual.SourceId);
            Assert.AreEqual(customerDataSource.DataSourceId, actual.DataSourceId);

        }

    }
}
