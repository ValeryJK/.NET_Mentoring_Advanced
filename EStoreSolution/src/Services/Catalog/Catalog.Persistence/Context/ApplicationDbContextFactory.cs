using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Catalog.Persistence.Context
{
	public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
			var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;

			builder.UseSqlServer(connectionString, options =>
				options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

			return new ApplicationDbContext(builder.Options);
		}
	}
}
