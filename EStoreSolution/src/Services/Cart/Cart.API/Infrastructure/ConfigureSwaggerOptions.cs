using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cart.API.Infrastructure
{
	/// <summary>
	/// Configures Swagger options for API versioning.
	/// </summary>
	public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
	{
		private readonly IApiVersionDescriptionProvider _provider;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
		/// </summary>
		/// <param name="provider">The API version description provider.</param>
		public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

		/// <summary>
		/// Configures Swagger options to create a Swagger document for each API version.
		/// </summary>
		/// <param name="options">The SwaggerGen options to configure.</param>
		public void Configure(SwaggerGenOptions options)
		{
			foreach (var description in _provider.ApiVersionDescriptions)
			{
				if (!options.SwaggerGeneratorOptions.SwaggerDocs.ContainsKey(description.GroupName))
				{
					options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
				}
			}
		}

		/// <summary>
		/// Creates OpenAPI info for a specific API version.
		/// </summary>
		/// <param name="description">The API version description.</param>
		/// <returns>An <see cref="OpenApiInfo"/> object with version details.</returns>
		private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
		{
			var info = new OpenApiInfo()
			{
				Title = $"Cart API {description.ApiVersion}",
				Version = description.ApiVersion.ToString(),
				Description = description.IsDeprecated ? "This API version is deprecated." : "API for managing carts and items.",
			};

			return info;
		}
	}
}