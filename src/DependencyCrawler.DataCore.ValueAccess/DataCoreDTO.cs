using System.Text.Json;

namespace DependencyCrawler.DataCore.ValueAccess;

public class DataCoreDTO : IValueDataCore
{
	public required Guid Id { get; init; }
	public required IReadOnlyDictionary<string, IValueModule> ModuleValues { get; init; }

	public static DataCoreDTO? Deserialize(string json)
	{
		return JsonSerializer.Deserialize<DataCoreDTO>(json, JsonSettings.GetSettings());
	}
}