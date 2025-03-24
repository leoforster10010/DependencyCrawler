using DependencyCrawler.DataCore.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.Data.Json;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddJsonDataSource(this IServiceCollection services)
	{
		services.AddTransient<IDataSource, JsonDataSource>();
		services.AddOptionsWithValidateOnStart<DataJsonSettings>()
			.BindConfiguration(nameof(DataJsonSettings))
			.ValidateDataAnnotations();

		return services;
	}
}