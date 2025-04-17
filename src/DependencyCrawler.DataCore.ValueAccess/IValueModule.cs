namespace DependencyCrawler.DataCore.ValueAccess;

public interface IValueModule
{
	string Name { get; }
	ModuleType Type { get; }
	IReadOnlyList<string> DependencyValues { get; }
	IReadOnlyList<string> ReferenceValues { get; }

	public ModuleDTO ToDTO()
	{
		return new ModuleDTO
		{
			Name = Name,
			Type = Type,
			DependencyValues = DependencyValues,
			ReferenceValues = ReferenceValues
		};
	}
}