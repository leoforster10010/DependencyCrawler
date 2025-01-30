using DependencyCrawler.CSharpCodeAnalysis;
using DependencyCrawler.DataCore;
using DependencyCrawler.QueryClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
	.ConfigureServices(services =>
	{
		services.AddTransient<IDependencyCrawler, QueryClient>();

		services.AddDataCore();
		services.AddCSharpCodeAnalysis();

		services.AddHostedService<Worker>();
	})
	.ConfigureLogging(_ => { })
	.ConfigureHostConfiguration(_ => { })
	.ConfigureAppConfiguration(configApp => { configApp.AddJsonFile("appsettings.json", true, true); })
	.Build()
	.Run();