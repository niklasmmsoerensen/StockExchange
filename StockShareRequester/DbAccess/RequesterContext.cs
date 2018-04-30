using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using StockShareRequester.Models;

namespace StockShareProvider.DbAccess
{
    public class RequesterContext : DbContext
    {
        public RequesterContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<BuyOrder> BuyOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }

    public class DesignTimeContext : IDesignTimeDbContextFactory<RequesterContext>
    {
        public RequesterContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<RequesterContext>();

            var connectionString = configuration.GetConnectionString("Default");

            builder.UseSqlServer(connectionString);

            return new RequesterContext(builder.Options);
        }
    }
}
