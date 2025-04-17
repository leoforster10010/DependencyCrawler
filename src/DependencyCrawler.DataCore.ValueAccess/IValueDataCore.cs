using System.Text.Json;

namespace DependencyCrawler.DataCore.ValueAccess;

public interface IValueDataCore
{
	Guid Id { get; }
	IReadOnlyDictionary<string, IValueModule> ModuleValues { get; }

	public string Serialize()
	{
		return JsonSerializer.Serialize(this, JsonSettings.GetSettings());
	}

	internal DataCoreDTO ToDTO()
	{
		return new DataCoreDTO
		{
			Id = Id,
			ModuleValues = ModuleValues
		};
	}
}