using System.Text.Json;

namespace DependencyCrawler.DataCore.ValueAccess;

public class ValueDataCore(IReadOnlyList<IValueModule> moduleValues, Guid id) : IValueDataCore
{
    public Guid Id { get; } = id;

    public IReadOnlyList<IValueModule> ModuleValues { get; } = moduleValues;

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
}