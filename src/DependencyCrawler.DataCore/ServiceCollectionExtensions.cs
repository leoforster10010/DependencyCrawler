using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.DataCore;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDataCore(this IServiceCollection services)
	{
		services.AddSingleton<DependencyCrawlerCoreProvider>();
		services.AddSingleton<IDependencyCrawlerCoreProvider>(x => x.GetRequiredService<DependencyCrawlerCoreProvider>());
		services.AddSingleton<IDependencyCrawlerDataAccess>(x => x.GetRequiredService<DependencyCrawlerCoreProvider>());
		services.AddSingleton<IDependencyCrawlerValueDataAccess>(x => x.GetRequiredService<DependencyCrawlerCoreProvider>());
		services.AddSingleton<IDependencyCrawlerReadonlyDataAccess>(x => x.GetRequiredService<DependencyCrawlerCoreProvider>());

		return services;
	}
}