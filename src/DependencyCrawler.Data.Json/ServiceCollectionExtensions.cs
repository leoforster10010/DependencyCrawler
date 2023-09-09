using DependencyCrawler.Data.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.Data.Json;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddJsonCache(this IServiceCollection services)
	{
		services.AddTransient<ICacher, JsonCacher>();

		return services;
	}
}