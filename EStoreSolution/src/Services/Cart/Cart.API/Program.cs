using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Cart.API.Infrastructure;
using Cart.Application;
using Cart.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
	.WithTracing(tracingBuilder =>
	{
		tracingBuilder
			.AddAspNetCoreInstrumentation()
			.AddHttpClientInstrumentation()
			.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Cart.API"))
			.AddOtlpExporter(options =>
			{
				var endpoint = builder.Configuration["OpenTelemetry:OtlpEndpoint"] ?? string.Empty;
				options.Endpoint = new Uri(endpoint);
			});
	});

builder.Services.AddCustomHealthChecks(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddApiVersioning(option =>
{
	option.DefaultApiVersion = new ApiVersion(1, 0);
	option.AssumeDefaultVersionWhenUnspecified = true;
	option.ReportApiVersions = true;
	option.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services
	.AddApiVersioning()
	.AddApiExplorer(options =>
	{

		options.GroupNameFormat = "'v'VVV";
		options.SubstituteApiVersionInUrl = true;
	});

builder.Services.Configure<SwaggerGenOptions>(options =>
{
	options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.RequireHttpsMetadata = false;
		options.Authority = builder.Configuration["Authentication:Authority"];
		options.Audience = builder.Configuration["Authentication:Audience"];
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
			ValidateAudience = false,
			ValidAudience = builder.Configuration["Authentication:Audience"],
			ValidateLifetime = true
		};
	});

builder.Services.AddAuthorizationBuilder()
	.AddPolicy("RoleAccessPolicy", policy =>
	{
		policy.RequireAssertion(context =>
			context.User.FindFirst("realm_access")?.Value?.Contains("manager") == true ||
			context.User.FindFirst("realm_access")?.Value?.Contains("store_customer") == true);
	});

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

app.MapHealthChecks("/hc", new HealthCheckOptions
{
	Predicate = _ => true,
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpMetrics();

app.UseMiddleware<TokenLoggingMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Test")
{
	app.UseDeveloperExceptionPage();

	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();
		foreach (var groupName in descriptions.Select(x => x.GroupName))
		{
			options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", $"Cart API {groupName.ToUpperInvariant()}");
		}
		options.RoutePrefix = "swagger";
	});
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.SeedDatabase();

await app.RunAsync();

/// <summary>
/// for integration tests
/// </summary>
public partial class Program
{
	private Program() { }
}
