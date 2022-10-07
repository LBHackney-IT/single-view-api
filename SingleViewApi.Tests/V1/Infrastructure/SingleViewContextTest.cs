using System.Linq;
using AutoFixture;
using NUnit.Framework;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.Tests.V1.Infrastructure;

[TestFixture]
public class SingleViewContextTest : DatabaseTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void CanGetADataSourceDbEntity()
    {
        var dataSourceDbEntity = _fixture.Create<DataSourceDbEntity>();

        SingleViewContext.Add(dataSourceDbEntity);
        SingleViewContext.SaveChanges();

        var result = SingleViewContext.DataSources.ToList().LastOrDefault();

        Assert.AreEqual(result.Id, dataSourceDbEntity.Id);
        Assert.AreEqual(result.Name, dataSourceDbEntity.Name);
    }
}
