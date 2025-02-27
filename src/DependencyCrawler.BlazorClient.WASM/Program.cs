using DependencyCrawler.BlazorClient.WASM;
using DependencyCrawler.CSharpCodeAnalysis.Client;
using DependencyCrawler.DataCore;
using DependencyCrawler.Framework;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddDataCore();
builder.Services.AddMudServices();
builder.AddCSharpCodeAnalysisClient();
builder.Logging.AddEventLogger(builder.Configuration);


await builder.Build().RunAsync();