using SingleViewApi.V1.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace SingleViewApi.Tests.V1.Domain
{
    [TestFixture]
    public class DataSourceTest
    {
        [Test]
        public void EntitiesHaveAnId()
        {
            var entity = new DataSource();
            entity.Id.Should().BeGreaterOrEqualTo(0);
        }

        [Test]
        public void EntitiesHaveAName()
        {
            var entity = new DataSource();
            var name = "My Entity";
            entity.Name = name;

            entity.Name.Should().Be(name);
        }
    }
}
