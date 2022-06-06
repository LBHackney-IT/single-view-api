using Microsoft.EntityFrameworkCore;

namespace SingleViewApi.V1.Infrastructure
{

    public class DatabaseContext : DbContext
    {
        //TODO: rename DatabaseContext to reflect the data source it is representing. eg. MosaicContext.
        //Guidance on the context class can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Database-contexts
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DataSourceEntity> DataSourceEntities { get; set; }
    }
}
