using DependencyCrawler.DataCore.DataAccess;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyCrawler.CSharpCodeAnalysis.Client;

public static class ServiceCollectionExtensions
{
	public static WebAssemblyHostBuilder AddCSharpCodeAnalysisClient(this WebAssemblyHostBuilder builder)
	{
		builder.Services.AddTransient(sp => new HttpClient
		{
			BaseAddress = new Uri("https://localhost:7157"),
			Timeout = TimeSpan.FromSeconds(60)
		});
		builder.Services.AddTransient<ICodeAnalysis, CSharpCodeAnalysisClient>();

		return builder;
	}
}