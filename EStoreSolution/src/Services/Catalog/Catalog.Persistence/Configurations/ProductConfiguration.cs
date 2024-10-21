using Catalog.Domain.Entities;

namespace Catalog.Persistence.Configurations
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.ToTable("Products", "dbo");

			builder.HasKey(e => e.Id);

			builder.Property(e => e.Name)
				  .IsRequired()
				  .HasMaxLength(50);

			builder.Property(e => e.Description)
				  .HasColumnType("nvarchar(max)");

			builder.Property(e => e.Image)
				  .HasMaxLength(2048);

			builder.Property(e => e.Price)
				  .HasColumnType("money")
				  .IsRequired();

			builder.Property(e => e.Amount)
				  .IsRequired();

			builder.HasOne(e => e.Category)
				  .WithMany(e => e.Products)
				  .HasForeignKey(e => e.CategoryId)
				  .OnDelete(DeleteBehavior.Cascade);

		}
	}
}
