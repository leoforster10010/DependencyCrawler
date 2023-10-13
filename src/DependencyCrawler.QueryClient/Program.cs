using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.QueryClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
	.ConfigureServices(services =>
	{
		services.AddTransient<IDependencyCrawler, DependencyCrawler.QueryClient.DependencyCrawler>();

		services.AddDependencyCrawler();

		services.AddHostedService<Worker>();
	})
	.ConfigureLogging(_ => { })
	.ConfigureHostConfiguration(_ => { })
	.ConfigureAppConfiguration(configApp => { configApp.AddJsonFile("appsettings.json", true, true); })
	.Build()
	.Run();