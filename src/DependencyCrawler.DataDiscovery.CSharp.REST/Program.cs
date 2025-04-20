using DependencyCrawler.DataDiscovery.CSharp;
using DependencyCrawler.Framework;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCors(options => { options.AddPolicy("PolicyName", corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin()); });
builder.Services.AddOpenApi();
builder.Services.AddTransient<IDataCoreDTOFactory, DataCoreDTOFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwaggerUI(c => { c.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1"); });
}

app.UseCors("PolicyName");

app.UseHttpsRedirection();

app.MapGet("/api/DataCoreDTO", (IDataCoreDTOFactory dataCoreDTOFactory, string? filePath) => dataCoreDTOFactory.CreateDataCoreDTO(filePath));

app.Run();