using Catalog.Domain.Entities;

namespace Catalog.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories", "dbo");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(50);

            builder.Property(e => e.Image)
                  .HasMaxLength(2048);

            builder.HasOne(e => e.ParentCategory)
                  .WithMany(e => e.ChildCategories)
                  .HasForeignKey(e => e.ParentCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
