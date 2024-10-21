using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application
{
	public static class ApplicationServiceRegistration
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddScoped<ICategoryService, CategoryService>();
			services.AddScoped<IProductService, ProductService>();

			return services;
		}
	}
}
