using Catalog.API.Controllers;

namespace Catalog.API.Resources
{
	public static class HateoasLinksFactory
	{
		public static Dictionary<string, (string ActionName, string Method)> GetCategoryLinks()
		{
			return new HateoasLinks()
				.AddLink("self", nameof(CategoriesController.GetCategoryById), "GET")
				.AddLink("update", nameof(CategoriesController.UpdateCategory), "PUT")
				.AddLink("delete", nameof(CategoriesController.DeleteCategory), "DELETE")
				.Build();
		}

		public static Dictionary<string, (string ActionName, string Method)> GetProductLinks()
		{
			return new HateoasLinks()
				.AddLink("self", nameof(ProductsController.GetProductById), "GET")
				.AddLink("update", nameof(ProductsController.UpdateProduct), "PUT")
				.AddLink("delete", nameof(ProductsController.DeleteProduct), "DELETE")
				.Build();
		}
	}
}
