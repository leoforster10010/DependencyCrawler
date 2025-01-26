using System.Text.Json;
using System.Text.Json.Serialization;

namespace DependencyCrawler.DataCore.ValueAccess;

public class DataCoreDTO
{
	[JsonConstructor]
	public DataCoreDTO(IReadOnlyList<ModuleDTO> moduleValues, Guid id)
	{
		Id = id;
		ModuleValues = moduleValues;
	}

	public DataCoreDTO(IValueDataCore valueDataCore)
	{
		Id = valueDataCore.Id;
		ModuleValues = valueDataCore.ModuleValues.Select(x => new ModuleDTO(x.ReferenceValues, x.DependencyValues, x.Name)).ToList();
	}

	public Guid Id { get; }

	public IReadOnlyList<ModuleDTO> ModuleValues { get; }

	public string Serialize()
	{
		return JsonSerializer.Serialize(this);
	}
}