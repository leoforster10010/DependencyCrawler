using DependencyCrawler.DataCore;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.Data.MongoDB;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMongoDbDataSource(this IServiceCollection services)
	{
		services.AddTransient<IDataSource, MongoDbDataSource>();

		return services;
	}
}