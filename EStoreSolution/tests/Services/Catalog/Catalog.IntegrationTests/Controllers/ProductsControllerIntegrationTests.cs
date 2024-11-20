using Catalog.IntegrationTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Catalog.IntegrationTests.Controllers
{
	public class ProductsControllerIntegrationTests : BaseIntegrationTest
	{
		public ProductsControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

		[Fact]
		public async Task GetPagedProducts_ReturnsOkResponse()
		{
			// Act
			var response = await _httpClient.GetAsync("/api/v1/products?pageNumber=1&pageSize=10");

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}
	}
}
