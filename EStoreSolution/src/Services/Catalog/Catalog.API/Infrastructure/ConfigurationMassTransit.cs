using MassTransit;

namespace Catalog.API.Infrastructure
{
	/// <summary>
	/// Configuration class for setting up MassTransit and RabbitMQ for the Catalog API.
	/// </summary>
	public static class ConfigurationMassTransit
	{
		/// <summary>
		/// Adds MassTransit configuration to the service collection.
		/// </summary>
		/// <param name="services">The IServiceCollection to configure.</param>
		public static void AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMassTransit(x =>
			{
				// Configuring RabbitMQ as the transport for MassTransit
				x.UsingRabbitMq((context, cfg) =>
				{
					// Retrieve RabbitMQ settings from the configuration
					var rabbitMqSettings = configuration.GetSection("RabbitMqSettings");
					var host = rabbitMqSettings["Host"];
					var username = rabbitMqSettings["Username"] ?? throw new InvalidOperationException("RabbitMQ username is not configured.");
					var password = rabbitMqSettings["Password"] ?? throw new InvalidOperationException("RabbitMQ password is not configured.");

					// Setting up the RabbitMQ host connection using settings from configuration
					cfg.Host(host, h =>
					{
						h.Username(username);
						h.Password(password);
					});

					// Configures all consumers that have been registered to use the endpoints
					cfg.ConfigureEndpoints(context);
				});
			});
		}
	}
}