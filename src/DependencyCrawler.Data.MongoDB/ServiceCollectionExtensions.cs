using DependencyCrawler.Data.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.Data.MongoDB;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMongoDbCache(this IServiceCollection services)
	{
		services.AddTransient<ICacher, MongoDbCacher>();
		services.AddSingleton<IRequiredConfigurations, RequiredMongoDbConfigurations>();

		return services;
	}
}