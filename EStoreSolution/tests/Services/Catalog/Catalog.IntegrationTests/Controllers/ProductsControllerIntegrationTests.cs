using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Catalog.IntegrationTests.Controllers
{
	public class ProductsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly HttpClient _client;

		public ProductsControllerIntegrationTests(WebApplicationFactory<Program> factory)
		{
			_client = factory.CreateClient();
		}

		[Fact]
		public async Task GetPagedProducts_ReturnsOkResponse()
		{
			// Act
			var response = await _client.GetAsync("/api/v1/products?pageNumber=1&pageSize=10");

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}
	}
}
