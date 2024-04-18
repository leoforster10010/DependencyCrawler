namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IReadOnlyModule
{
	string NameReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> DependingOnReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> DependencyOfReadOnly { get; }
}