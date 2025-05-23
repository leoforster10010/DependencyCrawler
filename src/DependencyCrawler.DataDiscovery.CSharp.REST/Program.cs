using DependencyCrawler.DataDiscovery.CSharp;
using DependencyCrawler.Framework;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCors();
builder.Services.AddOpenApi();
builder.Services.AddTransient<IDataCoreDTOFactory, DataCoreDTOFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors();

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.MapGet("/api/DataCoreDTO", (IDataCoreDTOFactory dataCoreDTOFactory, string? filePath) => dataCoreDTOFactory.CreateDataCoreDTO(filePath));

app.Run();