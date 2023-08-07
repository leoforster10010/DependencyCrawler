using System.Text.Json;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Models;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Data.Json;

public class JsonCacher : ICacher
{
	private readonly ILogger<JsonCacher> _logger;
	private readonly string _path = Directory.GetCurrentDirectory(); //ToDo

	public JsonCacher(ILogger<JsonCacher> logger)
	{
		_logger = logger;
	}

	public void SaveCaches(IEnumerable<Cache> caches)
	{
		DeleteOldFiles();

		foreach (var cache in caches)
		{
			var jsonPayload = JsonSerializer.Serialize(cache, new JsonSerializerOptions
			{
				WriteIndented = true
			});

			File.WriteAllText(jsonPayload, cache.Id + ".json");
		}
	}

	public IEnumerable<Cache> GetAvailableCaches()
	{
		var files = Directory.GetFiles(_path, ".json");
		var caches = new List<Cache>();

		foreach (var file in files)
		{
			var cache = TryLoadCache(file);
			if (cache is not null)
			{
				caches.Add(cache);
			}
		}

		return caches;
	}

	private void DeleteOldFiles()
	{
		var oldFiles = GetCacheFiles();
		foreach (var oldFile in oldFiles)
		{
			File.Delete(oldFile);
		}
	}

	private Cache? TryLoadCache(string file)
	{
		try
		{
			var jsonPayload = File.ReadAllText(file);
			var cache = JsonSerializer.Deserialize<Cache>(jsonPayload);
			return cache;
		}
		catch (Exception e)
		{
			_logger.LogError(e.ToString());
			return null;
		}
	}

	private IEnumerable<string> GetCacheFiles()
	{
		var files = Directory.GetFiles(_path, ".json");
		var cacheFiles = new List<string>();
		foreach (var file in files)
		{
			var cache = TryLoadCache(file);
			if (cache is not null)
			{
				cacheFiles.Add(file);
			}
		}

		return cacheFiles;
	}
}