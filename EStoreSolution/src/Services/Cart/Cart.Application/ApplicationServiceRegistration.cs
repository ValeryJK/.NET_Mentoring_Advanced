using Cart.Application.Interfaces;
using Cart.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cart.Application
{
	public static class ApplicationServiceRegistration
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddScoped<ICartService, CartService>();

			return services;
		}
	}
}
