using System.Text.Json;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.Data.Json;

public class JsonDataSource(IDataCoreProvider dataCoreProvider) : IDataSource
{
	private readonly string _path = Directory.GetCurrentDirectory();
	public Guid Id { get; } = Guid.NewGuid();

	public void Save()
	{
		//ToDo File-Handling
		File.WriteAllText(dataCoreProvider.ActiveCore.Id + "_" + ".json", dataCoreProvider.ActiveCore.ToDTO().Serialize());
	}

	public void Load()
	{
		//ToDo File-Handling
		var files = Directory.GetFiles(_path, "*.json");

		foreach (var file in files)
		{
			try
			{
				var jsonPayload = File.ReadAllText(file);
				//ToDo doesn't work
				var valueDataCore = JsonSerializer.Deserialize<DataCoreDTO>(jsonPayload);
				if (valueDataCore is not null)
				{
					dataCoreProvider.GetOrCreateDataCore(valueDataCore);
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}
	}
}