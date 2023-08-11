using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class ExternalProject : IExternalProject
{
	//ToDo: if InternalProject with same Name exists, link to it

	public IDictionary<string, IPackageReference> PackageReferences { get; set; } =
		new Dictionary<string, IPackageReference>();

	public required string Name { get; init; }

	public IDictionary<string, IReference> Dependencies
	{
		get
		{
			var allDependencies = new Dictionary<string, IReference>();
			foreach (var packageReference in PackageReferences)
			{
				allDependencies.TryAdd(packageReference.Key, packageReference.Value);
			}

			return allDependencies;
		}
	}

	public IDictionary<string, IReference> ReferencedBy { get; set; } = new Dictionary<string, IReference>();

	public ProjectType ProjectType => ProjectType.External;

	public IDictionary<string, IProjectNamespace> Namespaces { get; set; } =
		new Dictionary<string, IProjectNamespace>();

	public IDictionary<string, INamespaceType> Types
	{
		get
		{
			var projectTypes = new Dictionary<string, INamespaceType>();
			var types = Namespaces.Values.SelectMany(x => x.NamespaceTypes.Values);
			foreach (var type in types)
			{
				projectTypes.TryAdd(type.FullName, type);
			}

			return projectTypes;
		}
	}

	public IDictionary<string, ITypeUsingDirective> UsingDirectives
	{
		get
		{
			var projectUsingDirectives = new Dictionary<string, ITypeUsingDirective>();
			var usingDirectives = Namespaces.Values.SelectMany(x => x.TypeUsingDirectives.Values);
			foreach (var usingDirective in usingDirectives)
			{
				projectUsingDirectives.TryAdd(usingDirective.Name, usingDirective);
			}

			return projectUsingDirectives;
		}
	}

	public string NameReadOnly => Name;
	public ProjectType ProjectTypeReadOnly => ProjectType;

	public IReadOnlyDictionary<string, IReadOnlyReference> DependenciesReadOnly =>
		Dependencies.ToDictionary(x => x.Key, x => x.Value as IReadOnlyReference);

	public IReadOnlyDictionary<string, IReadOnlyReference> ReferencedByReadOnly =>
		ReferencedBy.ToDictionary(x => x.Key, x => x.Value as IReadOnlyReference);

	public IReadOnlyDictionary<string, IReadOnlyProjectNamespace> NamespacesReadOnly =>
		Namespaces.ToDictionary(x => x.Key, x => x.Value as IReadOnlyProjectNamespace);

	public IReadOnlyDictionary<string, IReadOnlyNamespaceType> TypesReadOnly =>
		Types.ToDictionary(x => x.Key, x => x.Value as IReadOnlyNamespaceType);

	public IReadOnlyDictionary<string, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly =>
		UsingDirectives.ToDictionary(x => x.Key, x => x.Value as IReadOnlyTypeUsingDirective);

	public IReadOnlyDictionary<string, IReadOnlyPackageReference> PackageReferencesReadOnly =>
		PackageReferences.ToDictionary(x => x.Key, x => x.Value as IReadOnlyPackageReference);
}