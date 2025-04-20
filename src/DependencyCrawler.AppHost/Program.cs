using DependencyCrawler.Framework;

var builder = DistributedApplication.CreateBuilder(args);

var postgresResource = builder.AddPostgres(Constants.PostgresResourceName)
    .WithPgAdmin()
    .WithDataVolume();
var postgresdb = postgresResource.AddDatabase(Constants.PostgresdbName);

var mongoResource = builder.AddMongoDB(Constants.MongoResourceName)
    .WithDataVolume();
var mongodb = mongoResource.AddDatabase(Constants.MongoDbName);

var postgresMigrationService =  builder.AddProject<Projects.DependencyCrawler_Data_Postgresql_MigrationService>(Constants.PostgresMigrationServiceName)
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

var cSharpRest = builder.AddProject<Projects.DependencyCrawler_DataDiscovery_CSharp_REST>(Constants.CSharpDataDiscoveryRestName);

builder.AddProject<Projects.DependencyCrawler_BlazorClient_Server>(Constants.BlazorClientName)
    .WithReference(mongodb)
    .WaitFor(mongodb)
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(postgresMigrationService)
    .WaitFor(postgresMigrationService)
    .WithReference(cSharpRest)
    .WaitFor(cSharpRest);

builder.Build().Run();
