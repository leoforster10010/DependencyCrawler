using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.DataCore;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDataCore(this IServiceCollection services)
	{
		services.AddSingleton<DataCoreProvider>();
		services.AddSingleton<IDataCoreProvider>(x => x.GetRequiredService<DataCoreProvider>());
		services.AddSingleton<IDataAccess>(x => x.GetRequiredService<DataCoreProvider>());
		services.AddSingleton<IValueDataAccess>(x => x.GetRequiredService<DataCoreProvider>());
		services.AddSingleton<IReadonlyDataAccess>(x => x.GetRequiredService<DataCoreProvider>());

		return services;
	}
}