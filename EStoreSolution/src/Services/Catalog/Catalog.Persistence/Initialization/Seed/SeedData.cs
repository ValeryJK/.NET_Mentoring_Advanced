using Catalog.Domain.Entities;

namespace Catalog.Persistence.Initialization.Seed
{
#pragma warning disable CS9035
	public static class SeedData
	{
		public static List<Product> Products => new List<Product>
		{
			new Product
			{
				Id = 1,
				Name = "Harry Potter",
				Description = "A famous book about a young wizard.",
				Price = 29.99m,
				Amount = 100,
				Image = "https://example.com/default-image.png",
				CategoryId = 1				
			},
			new Product
			{
				Id = 2,
				Name = "1984",
				Description = "A dystopian novel by George Orwell.",
				Price = 19.99m,
				Amount = 50,
				Image = "https://example.com/default-image.png",
				CategoryId = 2
			},
			new Product
			{
				Id = 3,
				Name = "T-shirt",
				Description = "A cotton t-shirt.",
				Price = 9.99m,
				Amount = 200,
				Image = "https://example.com/default-image.png",
				CategoryId = 3
			}
		};

		public static List<Category> Categories => new List<Category>
		{
			new Category { Id = 1, Name = "Books", Image = "https://example.com/default-image.png" },
			new Category { Id = 2, Name = "Clothing", Image = "https://example.com/default-image.png" },
			new Category { Id = 3, Name = "Electronics", Image = "https://example.com/default-image.png" }
		};
	}
}
#pragma warning restore CS9035
