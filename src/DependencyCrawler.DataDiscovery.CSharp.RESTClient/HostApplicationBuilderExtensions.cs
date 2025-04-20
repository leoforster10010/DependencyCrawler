using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyCrawler.DataDiscovery.CSharp.RESTClient;

public static class HostApplicationBuilderExtensions
{
	public static IHostApplicationBuilder AddCSharpCodeAnalysisClient(this IHostApplicationBuilder builder)
	{
		builder.Services.AddHttpClient<GetDataCoreDTOClient>(client =>
		{
			client.BaseAddress = new Uri($"https+http://{Constants.CSharpDataDiscoveryRestName}");
		});
		builder.Services.AddTransient<IDataDiscovery, CSharpDataDiscoveryClient>();

		return builder;
	}
}