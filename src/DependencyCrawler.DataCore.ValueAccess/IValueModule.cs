namespace DependencyCrawler.DataCore.ValueAccess;

public interface IValueModule
{
	string Name { get; }
	IReadOnlyList<string> DependencyValues { get; }
	IReadOnlyList<string> ReferenceValues { get; }

	public ModuleDTO ToDTO()
	{
		return new ModuleDTO
		{
			Name = Name,
			DependencyValues = DependencyValues,
			ReferenceValues = ReferenceValues
		};
	}
}