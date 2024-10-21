using Catalog.Domain.Entities;

namespace Catalog.Persistence.Context
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext() : base()
		{
		}

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options) { }		

		public DbSet<Category> Categories => Set<Category>();
		public DbSet<Product> Products => Set<Product>();

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=CatalogDb;Integrated Security=True;TrustServerCertificate=True",
					   builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
		}
	}
}
