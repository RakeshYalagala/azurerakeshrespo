using Microsoft.EntityFrameworkCore;

namespace Azureblob2.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
    : base(options)
        {
        }
        public DbSet<UserMaster> userMaster { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            //         modelBuilder.Entity<ProjectTeam>()
            // .ToTable("PROJECT_TEAM", "EOL");

            //modelBuilder.Entity<EolEcuMaster>()
            //    .HasIndex(e => new { e.EcuName, e.ProjectCode })
            //    .IsUnique();

            //modelBuilder.Entity<EolStatusMaster>()
            //.HasIndex(e => new { e.StatusCode, e.StatusType })
            //.IsUnique()
            //.HasDatabaseName("UQ_STATUS_CODE_TYPE");


        }

    }
}
