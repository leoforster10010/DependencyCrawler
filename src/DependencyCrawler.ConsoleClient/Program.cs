using DependencyCrawler.ConsoleClient;
using DependencyCrawler.Data.Json;
using DependencyCrawler.Framework.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
	.ConfigureServices(services =>
	{
		services.AddTransient<IConsoleClient, ConsoleClient>();

		services.AddDependencyCrawler();

		services.AddJsonCache();

		services.AddHostedService<Worker>();
	})
	.ConfigureLogging(_ => { })
	.ConfigureHostConfiguration(_ => { })
	.ConfigureAppConfiguration(configApp => { configApp.AddJsonFile("appsettings.json", true, true); })
	.Build()
	.Run();