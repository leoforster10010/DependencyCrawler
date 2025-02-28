using System.ComponentModel.DataAnnotations;

namespace DependencyCrawler.Data.Sqlite;

internal class DataSqliteSettings
{
	[Required] public required string DbPath { get; init; }
}