﻿using Cart.Application.Features.CreateCart;
using Cart.Application.Features.GetCart;
using Cart.Domain.Entities;
using Cart.IntegrationTests.Base;
using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Json;

namespace Cart.IntegrationTests
{
	public class CartsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
	{
		private readonly HttpClient _client;
		private readonly IMongoDatabase _database;

		public CartsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
		{
			_client = factory.CreateAuthenticatedClientAsync().GetAwaiter().GetResult();
			_database = factory.Database;
		}

		[Fact]
		public async Task GetCartInfo_ReturnsNotFound_WhenCartDoesNotExist()
		{
			// Arrange
			var cartId = "non-existent-cart-id";

			// Act
			var response = await _client.GetAsync($"/api/v1/carts/{cartId}");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task AddItem_AddsItemToCart_And_CanRetrieveIt()
		{
			// Arrange
			var cartId = Guid.NewGuid().ToString();
			var command = new CreateCartCommand
			{
				CartId = cartId,
				InitialItems = new List<CartItem>
				{
					new CartItem { Id = 1, Name = "Product 1", Quantity = 2, Price = 10.0m }
				}
			};

			// Act
			var postResponse = await _client.PostAsJsonAsync($"/api/v1/carts/{cartId}/items", command);
			postResponse.EnsureSuccessStatusCode();

			var getResponse = await _client.GetAsync($"/api/v1/carts/{cartId}");
			getResponse.EnsureSuccessStatusCode();

			var cart = await getResponse.Content.ReadFromJsonAsync<GetCartQueryResponse>();

			// Assert
			cart.Should().NotBeNull();
			cart!.Id.Should().Be(cartId);
			cart.CartItems.Should().HaveCount(1);
			cart.CartItems[0].Name.Should().Be("Product 1");
			cart.CartItems[0].Quantity.Should().Be(2);
			cart.CartItems[0].Price.Should().Be(10.0m);
		}

		[Fact]
		public async Task DeleteItem_RemovesItemFromCart()
		{
			// Arrange
			var cartId = Guid.NewGuid().ToString();
			var command = new CreateCartCommand
			{
				CartId = cartId,
				InitialItems = new List<CartItem>
				{
					new CartItem { Id = 1, Name = "Product 1", Quantity = 2, Price = 10.0m }
				}
			};

			var postResponse = await _client.PostAsJsonAsync($"/api/v1/carts/{cartId}/items", command);
			postResponse.EnsureSuccessStatusCode();

			// Act
			var deleteResponse = await _client.DeleteAsync($"/api/v1/carts/{cartId}/items/1");
			deleteResponse.EnsureSuccessStatusCode();

			var getResponse = await _client.GetAsync($"/api/v1/carts/{cartId}");
			getResponse.EnsureSuccessStatusCode();

			var cart = await getResponse.Content.ReadFromJsonAsync<GetCartQueryResponse>();

			// Assert
			cart.Should().NotBeNull();
			cart!.CartItems.Should().BeEmpty();
		}

		internal void Dispose()
		{
			_database.DropCollection("Carts");
		}
	}
}