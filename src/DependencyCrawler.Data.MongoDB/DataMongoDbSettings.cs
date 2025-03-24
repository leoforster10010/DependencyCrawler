﻿using System.ComponentModel.DataAnnotations;

namespace DependencyCrawler.Data.MongoDB;

internal class DataMongoDbSettings
{
	[Required] public required string ConnectionString { get; init; }
	[Required] public required string DatabaseName { get; init; }
}