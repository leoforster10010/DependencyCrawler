using System.ComponentModel.DataAnnotations;

namespace DependencyCrawler.Data.Json;

internal class DataJsonSettings
{
	[Required] public required string FilePath { get; init; }
}