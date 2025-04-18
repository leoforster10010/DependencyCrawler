﻿using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Data.Contracts.Interfaces;
using DependencyCrawler.Implementations.Data;
using DependencyCrawler.Implementations.Repositories;
using DependencyCrawler.Implementations.Repositories.DataAccess;
using DependencyCrawler.Implementations.Repositories.Factories;
using DependencyCrawler.Implementations.Repositories.Framework;
using DependencyCrawler.Implementations.Repositories.Loader;
using DependencyCrawler.Implementations.Repositories.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.Framework.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDependencyCrawler(this IServiceCollection services)
	{
		services.AddTransient<IProjectQueriesReadOnly, ProjectQueriesReadOnly>();
		services.AddTransient<IEvaluationRepository, EvaluationRepository>();

		services.AddSingleton<ICacheManager, CacheManager>();

		services.AddTransient<IProjectLoader, ProjectLoader>();
		services.AddTransient<ICachedProjectLoader, CachedProjectLoader>();

		services.AddTransient<IProjectInfoFactory, ProjectInfoFactory>();
		services.AddTransient<ILinkedTypeFactory, LinkedTypeFactory>();
		services.AddTransient<ICachedTypeFactory, CachedTypeFactory>();

		services.AddSingleton<IProjectProvider, ProjectProvider>();
		services.AddTransient<IReadOnlyProjectProvider, ReadOnlyProjectProvider>();
		services.AddSingleton<IProjectFileProvider, ProjectFileProvider>();
		services.AddSingleton<IDllFileProvider, DllFileProvider>();
		services.AddSingleton<ICachedProjectProvider, CachedProjectProvider>();

		services.AddSingleton<IRequiredConfigurations, RequiredConfigurations>();
		services.AddTransient<IConfigurationValidator, ConfigurationValidator>();

		return services;
	}
}