using System.Net.Http.Headers;
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

namespace Cart.IntegrationTests.Base
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private readonly MongoDbRunner mongoRunner;

        public IMongoDatabase Database { get; private set; }

        private readonly IConfiguration configuration;
        private string? accessToken;

        public CustomWebApplicationFactory()
        {
            this.mongoRunner = MongoDbRunner.Start();
            this.Database = new MongoClient(this.mongoRunner.ConnectionString).GetDatabase("TestDatabase");

            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ICartContext>();
                services.RemoveAll<ICartRepository>();
                services.AddSingleton<IMongoDatabase>(_ => this.Database);
                services.AddSingleton<ICartContext>(_ => new CartContext(new MongoClient(this.mongoRunner.ConnectionString), "TestDatabase", "Carts"));
                services.AddScoped<ICartRepository, CartRepository>();
            });
        }

        public async Task<HttpClient> CreateAuthenticatedClientAsync()
        {
            if (this.accessToken == null)
            {
                this.accessToken = await this.GetAccessTokenAsync();
            }

            var client = this.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.accessToken);
            return client;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            using var client = new HttpClient();
            var tokenRequest = new Dictionary<string, string>
            {
                { "client_id", this.configuration["Authentication:ClientId"] ?? throw new InvalidOperationException("Authentication ClientId is missing or incomplete.") },
                { "client_secret", "tK5iT9My7gt0n7PYMPJbznaSqmAFhbLI" },
                { "grant_type", this.configuration["Authentication:GrantType"] ?? throw new InvalidOperationException("Authentication GrantType is missing or incomplete.") },
                { "username", "test_user" },
                { "password", "11111" }
            };

            var response = await client.PostAsync($"{this.configuration["Authentication:Authority"]}/protocol/openid-connect/token", new FormUrlEncodedContent(tokenRequest));

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

            return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Failed to retrieve access token.");
        }

        public new void Dispose()
        {
            this.mongoRunner.Dispose();
        }

        private class TokenResponse
        {
            [JsonProperty("access_token")]
            public string? AccessToken { get; set; }
        }
    }
}
