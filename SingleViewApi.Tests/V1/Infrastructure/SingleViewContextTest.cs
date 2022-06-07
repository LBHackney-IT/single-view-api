using System.Linq;
using SingleViewApi.Tests.V1.Helper;
using NUnit.Framework;

namespace SingleViewApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class SingleViewContextTest : DatabaseTests
    {
        [Test]
        [Ignore("Bypass RDS tests")]
        public void CanGetADataSourceDbEntity()
        {
            var dataSourceDbEntity = DataSourceDbEntityHelper.CreateDatabaseEntity();

            SingleViewContext.Add(dataSourceDbEntity);
            SingleViewContext.SaveChanges();

            var result = SingleViewContext.DataSources.ToList().LastOrDefault();

            Assert.AreEqual(result.Id, dataSourceDbEntity.Id);
            Assert.AreEqual(result.Name, dataSourceDbEntity.Name);
        }
    }
}
