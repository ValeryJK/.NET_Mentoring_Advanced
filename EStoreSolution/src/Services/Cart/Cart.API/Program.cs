using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Cart.API.Infrastructure;
using Cart.Application;
using Cart.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Prometheus;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cart API", Version = "v1" });

	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	c.IncludeXmlComments(xmlPath);
});

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

app.MapHealthChecks("/hc", new HealthCheckOptions
{
	Predicate = _ => true,
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpMetrics();

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
