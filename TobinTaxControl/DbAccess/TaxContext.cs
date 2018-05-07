using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TobinTaxControl.DbAccess.Entities;

namespace TobinTaxControl.DbAccess
{
    public class TaxContext : DbContext
    {
        public TaxContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Taxation> Taxes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }

    public class DesignTimeContext : IDesignTimeDbContextFactory<TaxContext>
    {
        public TaxContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<TaxContext>();

            var connectionString = configuration.GetConnectionString("Default");

            builder.UseSqlServer(connectionString);

            return new TaxContext(builder.Options);
        }
    }
}
