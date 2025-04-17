var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DependencyCrawler_BlazorClient_Server>("dependencycrawler-blazorclient-server");

builder.Build().Run();
