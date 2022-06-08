using Microsoft.EntityFrameworkCore;

namespace SingleViewApi.V1.Infrastructure
{

    public class SingleViewContext : DbContext
    {
        //Guidance on the context class can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Database-contexts
        public SingleViewContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DataSourceDbEntity> DataSources { get; set; }
    }
}
