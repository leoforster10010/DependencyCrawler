﻿using DependencyCrawler.DataCore;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.CSharpCodeAnalysis;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCSharpCodeAnalysis(this IServiceCollection services)
	{
		services.AddTransient<ICodeAnalysis, CSharpCodeAnalysis>();

		return services;
	}
}