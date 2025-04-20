using System.Text.Json;

namespace DependencyCrawler.DataCore.ValueAccess;

internal static class JsonSettings
{
	internal static JsonSerializerOptions GetSettings()
	{
		return new JsonSerializerOptions
		{
			WriteIndented = true,
			PropertyNameCaseInsensitive = true,
			Converters =
			{
				new ValueCoreConverter(),
				new ValueModuleConverter()
			}
		};
	}
}