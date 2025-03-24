using System.Text.Json;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Data.Json;

public class JsonDataSource(IDataCoreProvider dataCoreProvider, ILogger<JsonDataSource> logger, IConfiguration configuration) : IDataSource
{
	private readonly string _directoryPath = configuration.GetSection(nameof(DataJsonSettings)).Get<DataJsonSettings>()!.FilePath;

	public Guid Id { get; } = Guid.NewGuid();

	public async Task Save()
	{
		var filePath = Path.Combine(_directoryPath, $"{dataCoreProvider.ActiveCore.Id}.json");
		await File.WriteAllTextAsync(filePath, dataCoreProvider.ActiveCore.Serialize());
	}

	public async Task Load()
	{
		if (!Directory.Exists(_directoryPath))
		{
			logger.LogError($"The directory '{_directoryPath}' does not exist.");
			return;
		}

		var jsonFiles = Directory.GetFiles(_directoryPath, "*.json");
		foreach (var file in jsonFiles)
		{
			try
			{
				var json = await File.ReadAllTextAsync(file);
				var dataCoreDto = DataCoreDTO.Deserialize(json);
				if (dataCoreDto != null)
				{
					dataCoreProvider.GetOrCreateDataCore(dataCoreDto);
				}
			}
			catch (JsonException)
			{
				logger.LogWarning($"Failed to deserialize JSON file '{file}'");
			}
		}
	}
}