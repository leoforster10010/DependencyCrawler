using DependencyCrawler.Data.Contracts.Entities;
using DependencyCrawler.Data.Contracts.Enum;
using DependencyCrawler.Data.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.Data.MongoDB;

internal class RequiredMongoDbConfigurations : IRequiredConfigurations
{
	public IReadOnlyDictionary<ConfigurationKeys, ConfigurationTypes> Entries =>
		new Dictionary<ConfigurationKeys, ConfigurationTypes>
		{
			{ ConfigurationKeys.RootDirectory, ConfigurationTypes.Path },
			{ ConfigurationKeys.DllDirectory, ConfigurationTypes.Path }
		};
}

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMongoDbCache(this IServiceCollection services)
	{
		services.AddTransient<ICacher, MongoDbCacher>();
		services.AddSingleton<IRequiredConfigurations, RequiredMongoDbConfigurations>();

		return services;
	}
}

public class MongoDbCacher : ICacher
{
	public void SaveCaches(IEnumerable<Cache> caches)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<Cache> GetAvailableCaches()
	{
		throw new NotImplementedException();
	}
}