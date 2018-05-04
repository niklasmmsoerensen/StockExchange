using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PublicShareOwnerControl.DbAccess.Entities;

namespace PublicShareOwnerControl.DbAccess
{
    public class OwnerControlContext : DbContext
    {
        public OwnerControlContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Stock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }

    public class DesignTimeContext : IDesignTimeDbContextFactory<OwnerControlContext>
    {
        public OwnerControlContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<OwnerControlContext>();

            var connectionString = configuration.GetConnectionString("Default");

            builder.UseSqlServer(connectionString);

            return new OwnerControlContext(builder.Options);
        }
    }
}
