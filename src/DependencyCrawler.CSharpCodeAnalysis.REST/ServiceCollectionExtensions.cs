using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.CSharpCodeAnalysis.REST;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCSharpCodeAnalysis(this IServiceCollection services)
	{
		services.AddTransient<ICodeAnalysis, CSharpCodeAnalysis>();
		services.AddTransient<IDataCoreDTOFactory, DataCoreDTOFactory>();

		return services;
	}
}