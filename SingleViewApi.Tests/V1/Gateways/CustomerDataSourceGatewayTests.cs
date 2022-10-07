using System;
using System.Linq;
using AutoFixture;
using NUnit.Framework;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.Tests.V1.Gateways;

[TestFixture]
public class CustomerDataSourceGatewayTests : DatabaseTests
{
    [SetUp]
    public void Setup()
    {
        _classUnderTest = new CustomerDataSourceGateway(SingleViewContext);
    }

    private readonly Fixture _fixture = new();
    private CustomerDataSourceGateway _classUnderTest;

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
