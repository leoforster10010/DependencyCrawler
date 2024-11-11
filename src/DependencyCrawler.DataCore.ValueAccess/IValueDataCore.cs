namespace DependencyCrawler.DataCore.ValueAccess;

public interface IValueDataCore
{
    Guid Id { get; }
    IReadOnlyList<IValueModule> ModuleValues { get; }
    string Serialize();
}