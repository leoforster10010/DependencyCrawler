using System.Text.Json;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

public class JsonDataSource : IDataSource
{
	//ToDo configurable directory
	private readonly string _path = Directory.GetCurrentDirectory();

	public void SaveCores(HashSet<IValueDataCore> dataCores)
	{
		DeleteOldFiles();

		var jsonSerializerOptions = new JsonSerializerOptions
		{
			WriteIndented = true
		};

		foreach (var valueDataCore in dataCores)
		{
			var jsonPayload = JsonSerializer.Serialize(valueDataCore, jsonSerializerOptions);

			File.WriteAllText(valueDataCore.Id + ".json", jsonPayload);
		}
	}

	public HashSet<IValueDataCore> LoadCores()
	{
		var files = Directory.GetFiles(_path, "*.json");
		var valueDataCores = new HashSet<IValueDataCore>();

		foreach (var file in files)
		{
			var cache = TryLoadValueDataCore(file);
			if (cache is not null)
			{
				valueDataCores.Add(cache);
			}
		}

		return valueDataCores;
	}

	public Guid Id { get; } = Guid.NewGuid();

	private void DeleteOldFiles()
	{
		var oldFiles = GetValueDataCoreFiles();
		foreach (var oldFile in oldFiles)
		{
			File.Delete(oldFile);
		}
	}

	private static IValueDataCore? TryLoadValueDataCore(string file)
	{
		try
		{
			var jsonPayload = File.ReadAllText(file);
			var cache = JsonSerializer.Deserialize<IValueDataCore>(jsonPayload);
			return cache;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private IEnumerable<string> GetValueDataCoreFiles()
	{
		var files = Directory.GetFiles(_path, "*.json");
		var cacheFiles = new List<string>();
		foreach (var file in files)
		{
			var cache = TryLoadValueDataCore(file);
			if (cache is not null)
			{
				cacheFiles.Add(file);
			}
		}

		return cacheFiles;
	}
}