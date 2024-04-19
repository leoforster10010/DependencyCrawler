namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IReadOnlyModule
{
	string NameReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> DependenciesReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> ReferencesReadOnly { get; }
}