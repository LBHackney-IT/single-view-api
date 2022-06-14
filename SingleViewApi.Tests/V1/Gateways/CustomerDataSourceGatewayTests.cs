using System;
using System.Linq;
using AutoFixture;
using SingleViewApi.V1.Gateways;
using NUnit.Framework;

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
            var dummyCustomerId = _fixture.Create<Guid>();
            var dummyDataSourceId = _fixture.Create<int>();
            var dummySourceId = _fixture.Create<string>();

            _ = _classUnderTest.Add(dummyCustomerId, dummyDataSourceId, dummySourceId);

            var actual = SingleViewContext.CustomerDataSources.ToList().LastOrDefault();

            Assert.AreEqual(dummyCustomerId, actual.CustomerDbEntityId);
            Assert.AreEqual(dummySourceId, actual.SourceId);
            Assert.AreEqual(dummyDataSourceId, actual.DataSourceId);
        }

    }
}
