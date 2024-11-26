using Cart.Domain.Interfaces;
using Cart.Persistence.Context;
using Cart.Persistence.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mongo2Go;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Cart.IntegrationTests.Base
{
	public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
	{
		private readonly MongoDbRunner _mongoRunner;
		public IMongoDatabase Database { get; private set; }
		private readonly IConfiguration _configuration;
		private string? _accessToken;

		public CustomWebApplicationFactory()
		{
			_mongoRunner = MongoDbRunner.Start();
			Database = new MongoClient(_mongoRunner.ConnectionString).GetDatabase("TestDatabase");

			_configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Test");

			builder.ConfigureServices(services =>
			{
				services.RemoveAll<ICartContext>();
				services.RemoveAll<ICartRepository>();
				services.AddSingleton<IMongoDatabase>(_ => Database);
				services.AddSingleton<ICartContext>(_ => new CartContext(new MongoClient(_mongoRunner.ConnectionString), "TestDatabase", "Carts"));
				services.AddScoped<ICartRepository, CartRepository>();
			});
		}

		public async Task<HttpClient> CreateAuthenticatedClientAsync()
		{
			if (_accessToken == null)
				_accessToken = await GetAccessTokenAsync();

			var client = CreateClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
			return client;
		}

		private async Task<string> GetAccessTokenAsync()
		{
			using var client = new HttpClient();
			var tokenRequest = new Dictionary<string, string>
			{
				{ "client_id", _configuration["Authentication:ClientId"] ?? throw new InvalidOperationException("Authentication ClientId is missing or incomplete.") },
				{ "client_secret", "tK5iT9My7gt0n7PYMPJbznaSqmAFhbLI" },
				{ "grant_type", _configuration["Authentication:GrantType"] ?? throw new InvalidOperationException("Authentication GrantType is missing or incomplete.") },
				{ "username", "test_user" },
				{ "password", "11111" }
			};

			var response = await client.PostAsync($"{_configuration["Authentication:Authority"]}/protocol/openid-connect/token", new FormUrlEncodedContent(tokenRequest));

			response.EnsureSuccessStatusCode();

			var responseBody = await response.Content.ReadAsStringAsync();
			var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

			return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Failed to retrieve access token.");
		}

		public new void Dispose()
		{
			_mongoRunner.Dispose();
		}

		private class TokenResponse
		{
			[JsonProperty("access_token")]
			public string? AccessToken { get; set; }
		}
	}
}
