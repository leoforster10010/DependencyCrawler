using System.Text.Json;
using System.Text.Json.Serialization;

namespace DependencyCrawler.DataCore.ValueAccess;

internal class ValueCoreConverter : JsonConverter<IValueDataCore>
{
	public override IValueDataCore Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return JsonSerializer.Deserialize<DataCoreDTO>(ref reader, options)!;
	}

	public override void Write(Utf8JsonWriter writer, IValueDataCore value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, value.ToDTO(), options);
	}
}