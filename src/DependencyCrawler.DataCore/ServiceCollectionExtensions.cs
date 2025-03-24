using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.DataCore;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDataCore(this IServiceCollection services)
	{
		services.AddSingleton<IDataCoreProvider, DataCoreProvider>();
		services.AddScoped<IDataSourceProvider, DataSourceProvider>();
		services.AddSingleton<ICodeAnalysisProvider, CodeAnalysisProvider>();

		services.AddSingleton<IReadOnlyDataAccess>(sp => sp.GetService<IDataCoreProvider>()!);
		services.AddSingleton<IValueDataAccess>(sp => sp.GetService<IDataCoreProvider>()!);
		services.AddSingleton<IValueDataAccess>(sp => sp.GetService<IDataCoreProvider>()!);

		return services;
	}
}