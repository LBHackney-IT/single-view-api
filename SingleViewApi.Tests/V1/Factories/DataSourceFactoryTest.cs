using AutoFixture;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Factories;
using SingleViewApi.V1.Infrastructure;
using NUnit.Framework;

namespace SingleViewApi.Tests.V1.Factories
{
    [TestFixture]
    public class DataSourceFactoryTest
    {
        private readonly Fixture _fixture = new();

        [Test]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var databaseEntity = _fixture.Create<DataSourceDbEntity>();
            var entity = databaseEntity.ToDomain();

            Assert.AreEqual(databaseEntity.Id, entity.Id);
            Assert.AreEqual(databaseEntity.Name, entity.Name);
        }

        [Test]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var entity = _fixture.Create<DataSource>();
            var databaseEntity = entity.ToDatabase();

            Assert.AreEqual(entity.Id, databaseEntity.Id);
            Assert.AreEqual(entity.Name, databaseEntity.Name);
        }
    }
}
