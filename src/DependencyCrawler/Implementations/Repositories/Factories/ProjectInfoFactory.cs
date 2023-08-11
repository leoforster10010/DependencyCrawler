using System.Reflection;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;
using Microsoft.Build.Construction;
using TypeInfo = DependencyCrawler.Implementations.Models.UnlinkedTypes.TypeInfo;

namespace DependencyCrawler.Implementations.Repositories.Factories;

internal class ProjectInfoFactory : IProjectInfoFactory
{
	private readonly IProjectFileProvider _projectFileProvider;

	public ProjectInfoFactory(IProjectFileProvider projectFileProvider)
	{
		_projectFileProvider = projectFileProvider;
	}

	public InternalProjectInfo GetInternalProjectInfo(string csprojFilePath)
	{
		var projectRootElement = GetProjectRootElement(csprojFilePath);

		var name = projectRootElement.FullPath.GetProjectName();
		var packageReferences = projectRootElement.GetPackageReferences().ToList();
		var projectReferences = projectRootElement.GetProjectReferences().ToList();
		var namespaces = GetNamespaces(name).ToList();

		return new InternalProjectInfo
		{
			Name = name,
			Namespaces = namespaces,
			PackageReferences = packageReferences,
			ProjectReferences = projectReferences
		};
	}

	public ExternalProjectInfo GetExternalProjectInfo(string dllFilePath)
	{
		var assembly = GetAssembly(dllFilePath);
		if (assembly is null)
		{
			//ToDo: if failed try other dll?
			return new ExternalProjectInfo
			{
				Name = dllFilePath.GetDllName()
			};
		}

		var name = assembly.GetProjectName();
		var packageReferences = assembly.GetPackageReferenceInfos().ToList();
		var namespaces = GetNamespaces(assembly);

		return new ExternalProjectInfo
		{
			Name = name,
			Namespaces = namespaces,
			PackageReferences = packageReferences
		};
	}

	private ProjectRootElement GetProjectRootElement(string csprojFilePath)
	{
		return ProjectRootElement.Open(csprojFilePath) ??
		       throw new ArgumentException($"Invalid Path supplied:{csprojFilePath}");
	}

	private Assembly? GetAssembly(string dllFilePath)
	{
		try
		{
			return Assembly.LoadFile(dllFilePath);
		}
		catch
		{
			//ignored
			return null;
		}
	}

	private IList<NamespaceInfo> GetNamespaces(Assembly assembly)
	{
		var namespaces = assembly.GetNamespaces().ToList();
		foreach (var ns in namespaces)
		{
			ns.Types = GetTypeInfos(assembly, ns);
		}

		return namespaces;
	}

	private IList<TypeInfo> GetTypeInfos(Assembly assembly, NamespaceInfo ns)
	{
		var types = assembly.GetTypesSafe().Where(x => x.Namespace == ns.Name).Select(x => new TypeInfo
		{
			Name = x.Name
		}).ToList();
		return types;
	}


	private IEnumerable<NamespaceInfo> GetNamespaces(string projectName)
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
			namespaceInfo.Types.Add(GetTypeInfo(typeFile));
		}

		return namespaces;
	}

	private TypeInfo GetTypeInfo(string typeFile)
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