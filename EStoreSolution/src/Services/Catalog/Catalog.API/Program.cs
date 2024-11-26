using Catalog.API.Infrastructure;
using Catalog.Application;
using Catalog.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
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
			.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Catalog.API"))
			.AddOtlpExporter(options =>
			{
				var endpoint = builder.Configuration["OpenTelemetry:OtlpEndpoint"] ?? string.Empty;
				options.Endpoint = new Uri(endpoint);
			});
	});

builder.Services.AddSqlServerHealthCheck(builder.Configuration);

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.AddInfrastructure();

builder.Services.AddControllers();
builder.Services.AddMassTransitConfiguration(builder.Configuration);
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
	.AddPolicy("ReadAccessPolicy", policy =>
	{
		policy.RequireAssertion(context =>
			context.User.FindFirst("realm_access")?.Value?.Contains("manager") == true ||
			context.User.FindFirst("realm_access")?.Value?.Contains("store_customer") == true);
	})
	.AddPolicy("CreateAccessPolicy", policy =>
	{
		policy.RequireAssertion(context =>
			context.User.FindFirst("realm_access")?.Value?.Contains("manager") == true &&
			context.User.HasClaim(c => c.Type == "scope" && c.Value.Contains("create")));
	})
	.AddPolicy("UpdateAccessPolicy", policy =>
	{
		policy.RequireAssertion(context =>
			context.User.FindFirst("realm_access")?.Value?.Contains("manager") == true &&
			context.User.HasClaim(c => c.Type == "scope" && c.Value.Contains("update")));
	})
	.AddPolicy("DeleteAccessPolicy", policy =>
	{
		policy.RequireAssertion(context =>
			context.User.FindFirst("realm_access")?.Value?.Contains("manager") == true &&
			context.User.HasClaim(c => c.Type == "scope" && c.Value.Contains("delete")));
	});

var app = builder.Build();

await app.ConfigureDatabaseAsync();

app.MapHealthChecks("/hc", new HealthCheckOptions
{
	Predicate = _ => true,
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseMiddleware<TokenLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program
{
	private Program() { }
}
