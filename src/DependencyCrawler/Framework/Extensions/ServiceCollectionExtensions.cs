using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Repositories;
using DependencyCrawler.Implementations.Repositories.Factories;
using DependencyCrawler.Implementations.Repositories.Loader;
using DependencyCrawler.Implementations.Repositories.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.Framework.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDependencyCrawler(this IServiceCollection services)
	{
		services.AddTransient<IProjectQueries, ProjectQueries>();

		services.AddTransient<IInternalProjectInfoLoader, InternalProjectInfoLoader>();
		services.AddTransient<IExternalProjectInfoLoader, ExternalProjectInfoLoader>();
		services.AddTransient<IProjectLoader, ProjectLoader>();

		services.AddTransient<IProjectInfoFactory, ProjectInfoFactory>();
		services.AddTransient<ILinkedTypeFactory, LinkedTypeFactory>();

		services.AddSingleton<IProjectProvider, ProjectProvider>();
		services.AddSingleton<IProjectFileProvider, ProjectFileProvider>();
		services.AddSingleton<IDllFileProvider, DllFileProvider>();

		return services;
	}
}