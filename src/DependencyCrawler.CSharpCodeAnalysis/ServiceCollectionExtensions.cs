using DependencyCrawler.DataCore.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.CSharpCodeAnalysis;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCSharpCodeAnalysis(this IServiceCollection services)
	{
		services.AddTransient<ICodeAnalysis, CSharpCodeAnalysis>();
		services.AddTransient<IDataCoreDTOFactory, DataCoreDTOFactory>();
		services.AddOptionsWithValidateOnStart<CSharpCodeAnalysisSettings>()
			.BindConfiguration(nameof(CSharpCodeAnalysisSettings))
			.ValidateDataAnnotations();
		return services;
	}
}