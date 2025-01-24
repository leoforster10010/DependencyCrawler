using System.Text.Json;

namespace DependencyCrawler.DataCore.ValueAccess;

public class DataCoreDTO(IReadOnlyList<ModuleDTO> moduleValues, Guid id)
{
	public DataCoreDTO(IValueDataCore valueDataCore) : this(valueDataCore.ModuleValues.Select(x => new ModuleDTO(x.ReferenceValues, x.DependencyValues, x.Name)).ToList(), valueDataCore.Id)
	{
	}

	public Guid Id { get; } = id;

	public IReadOnlyList<ModuleDTO> ModuleValues { get; } = moduleValues;

	public string Serialize()
	{
		return JsonSerializer.Serialize(this);
	}
}