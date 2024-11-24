using Cart.API.Consumers;
using MassTransit;

namespace Cart.API.Infrastructure
{
	/// <summary>
	/// Configuration class for setting up MassTransit and RabbitMQ for the Cart API.
	/// </summary>
	public static class ConfigurationMassTransit
	{
		/// <summary>
		/// Adds MassTransit configuration to the service collection.
		/// </summary>
		/// <param name="services">The IServiceCollection to configure.</param>
		/// <param name="configuration"></param>
		public static void AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMassTransit(x =>
			{
				// Registering the consumers
				x.AddConsumer<CartItemUpdatedConsumer>();
				x.AddConsumer<DeadLetterConsumer>();

				// Configuring RabbitMQ as the transport for MassTransit
				x.UsingRabbitMq((context, cfg) =>
				{
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

					// Configuring a receive endpoint for cart-item-updates
					cfg.ReceiveEndpoint("cart-item-updates", e =>
					{
						// Associating the consumer with the endpoint
						e.ConfigureConsumer<CartItemUpdatedConsumer>(context);
						// Setting up retry policy for message handling
						e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
						// Configuring delayed redelivery intervals
						e.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5)));
					});

					// Configuring a receive endpoint for dead letter queue handling
					cfg.ReceiveEndpoint("cart-updates-dead-letter-queue", e =>
					{
						// Associating the consumer for handling dead-lettered messages
						e.ConfigureConsumer<DeadLetterConsumer>(context);
					});
				});
			});
		}
	}
}