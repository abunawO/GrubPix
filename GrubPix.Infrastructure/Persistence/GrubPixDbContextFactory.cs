using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace GrubPix.Infrastructure.Persistence
{
    public class GrubPixDbContextFactory : IDesignTimeDbContextFactory<GrubPixDbContext>
    {
        public GrubPixDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<GrubPixDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            return new GrubPixDbContext(optionsBuilder.Options);
        }
    }
}
