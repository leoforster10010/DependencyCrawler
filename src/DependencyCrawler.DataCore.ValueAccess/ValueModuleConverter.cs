using System.Text.Json;
using System.Text.Json.Serialization;

namespace DependencyCrawler.DataCore.ValueAccess;

internal class ValueModuleConverter : JsonConverter<IValueModule>
{
	public override IValueModule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return JsonSerializer.Deserialize<ModuleDTO>(ref reader, options)!;
	}

	public override void Write(Utf8JsonWriter writer, IValueModule value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, value.ToDTO(), options);
	}
}