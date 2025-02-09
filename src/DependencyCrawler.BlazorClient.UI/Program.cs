using DependencyCrawler.BlazorClient.UI;
using DependencyCrawler.CSharpCodeAnalysis.Client;
using DependencyCrawler.DataCore;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddDataCore();
builder.Services.AddMudServices();
builder.AddCSharpCodeAnalysisClient();

await builder.Build().RunAsync();