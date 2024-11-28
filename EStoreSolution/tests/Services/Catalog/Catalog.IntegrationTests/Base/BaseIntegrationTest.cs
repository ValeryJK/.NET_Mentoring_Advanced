using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Catalog.IntegrationTests.Base
{
    public abstract class BaseIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly HttpClient httpClient;
        protected readonly IConfiguration configuration;

        protected BaseIntegrationTest(WebApplicationFactory<Program> factory)
        {
            this.httpClient = factory.CreateClient();
            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.GetAccessTokenAsync().Result);
        }

        protected async Task<string?> GetAccessTokenAsync()
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

            var response = await client.PostAsync(
                $"{this.configuration["Authentication:Authority"]}/protocol/openid-connect/token",
                new FormUrlEncodedContent(tokenRequest));
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

            return token?.AccessToken;
        }

        protected StringContent CreateJsonContent<T>(T data)
        {
            return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }
    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonProperty("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("scope")]
        public string? Scope { get; set; }
    }
}