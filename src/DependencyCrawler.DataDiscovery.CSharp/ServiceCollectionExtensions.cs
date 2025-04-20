using DependencyCrawler.DataCore.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.DataDiscovery.CSharp;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCSharpCodeAnalysis(this IServiceCollection services)
	{
		services.AddTransient<IDataDiscovery, CSharpDataDiscovery>();
		services.AddTransient<IDataCoreDTOFactory, DataCoreDTOFactory>();
		services.AddOptionsWithValidateOnStart<CSharpDataDiscoverySettings>()
			.BindConfiguration(nameof(CSharpDataDiscoverySettings))
			.ValidateDataAnnotations();
		return services;
	}
}