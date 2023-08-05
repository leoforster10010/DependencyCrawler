using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Implementations.Repositories.Loader;

public class InternalProjectInfoLoader : IInternalProjectInfoLoader
{
	private readonly IProjectFileProvider _projectFileProvider;

	public InternalProjectInfoLoader(IProjectFileProvider projectFileProvider)
	{
		_projectFileProvider = projectFileProvider;
	}

	public IEnumerable<NamespaceInfo> LoadNamespaces(string projectName)
	{
		var typeFiles = GetProjectTypes(projectName);
		var namespaces = new List<NamespaceInfo>();

		foreach (var typeFile in typeFiles)
		{
			var ns = GetNamespace(typeFile, projectName);

			if (namespaces.All(x => x.Name != ns.Name))
			{
				namespaces.Add(ns);
			}

			var namespaceInfo = namespaces.First(x => x.Name == ns.Name);
			namespaceInfo.Types.Add(LoadTypeInfo(typeFile));
		}

		return namespaces;
	}

	private TypeInfo LoadTypeInfo(string typeFile)
	{
		var typeInfo = new TypeInfo
		{
			Name = typeFile.GetClassName(),
			UsingDirectives = GetUsingDirectivesForType(typeFile)
		};

		return typeInfo;
	}

	private IList<UsingDirectiveInfo> GetUsingDirectivesForType(string typeFile)
	{
		var usingDirectives = File.ReadAllLines(typeFile)
			.Where(x => x.StartsWith("using"))
			.Select(x => new UsingDirectiveInfo
			{
				Namespace = x.GetUsingDirective()
			});

		return usingDirectives.ToList();
	}

	private NamespaceInfo GetNamespace(string typeFile, string projectName)
	{
		var namespaceLine = File.ReadAllLines(typeFile).FirstOrDefault(x => x.StartsWith("namespace")) ?? projectName;

		var namespaceString = namespaceLine.Remove("namespace ").Remove(";").Trim();

		return new NamespaceInfo
		{
			Name = namespaceString
		};
	}

	private IEnumerable<string> GetProjectTypes(string projectName)
	{
		const string extension = "*.cs";
		var directory = _projectFileProvider.GetProjectDirectory(projectName);

		return directory != null
			? Directory.GetFiles(directory, extension, SearchOption.AllDirectories)
			: Enumerable.Empty<string>();
	}
}