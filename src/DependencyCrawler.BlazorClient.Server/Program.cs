using DependencyCrawler.DataDiscovery.CSharp;
using DependencyCrawler.Data.Json;
using DependencyCrawler.Data.MongoDB;
using DependencyCrawler.Data.Postgresql;
using DependencyCrawler.DataCore;
using DependencyCrawler.DataDiscovery.CSharp.RESTClient;
using DependencyCrawler.Framework;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddDataCore();
builder.Logging.AddEventLogger(builder.Configuration);


builder.Services.AddCSharpCodeAnalysis();
builder.AddCSharpCodeAnalysisClient();

builder.AddMongoDbDataSource();
builder.AddPostgresqlDataSource();
builder.Services.AddJsonDataSource();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();