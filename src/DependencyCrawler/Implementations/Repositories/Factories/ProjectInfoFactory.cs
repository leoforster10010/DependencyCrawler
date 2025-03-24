using System.Reflection;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Data.Contracts.Enum;
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
		var types = assembly.GetTypesSafe().ToList();
		var namespaces = assembly.GetNamespaces(types).ToList();
		foreach (var ns in namespaces)
		{
			ns.Types = GetTypeInfos(types, ns);
		}

		return namespaces;
	}

	private IList<TypeInfo> GetTypeInfos(IEnumerable<Type> types, NamespaceInfo ns)
	{
		var typeInfos = types.Where(x => x.GetNamespace() == ns.Name).Select(x => new TypeInfo
		{
			Name = x.Name,
			FileType = FileType.CSharp
		}).ToList();
		return typeInfos;
	}


	private IEnumerable<NamespaceInfo> GetNamespaces(string projectName)
	{
		var typeInfos = GetTypeInfos(projectName);
		var namespaces = new List<NamespaceInfo>();

		foreach (var typeInfo in typeInfos)
		{
			var ns = GetNamespace(typeInfo.FileName, projectName);

			if (namespaces.All(x => x.Name != ns.Name))
			{
				namespaces.Add(ns);
			}

			var namespaceInfo = namespaces.First(x => x.Name == ns.Name);
			namespaceInfo.Types.Add(typeInfo);
		}

		return namespaces;
	}

	private IList<UsingDirectiveInfo> GetUsingDirectivesForType(string typeFile, FileType fileType)
	{
		var fileContent = File.ReadAllLines(typeFile);

		switch (fileType)
		{
			case FileType.CSharp:
				return fileContent.Where(x => x.StartsWith("using"))
					.Select(x => new UsingDirectiveInfo
					{
						Namespace = x.GetUsingDirective()
					}).ToList();
			case FileType.Xaml:
				return new List<UsingDirectiveInfo>();
			//ToDo
			//return fileContent.Where(x => x.StartsWith("using"))
			//	.Select(x => new UsingDirectiveInfo
			//	{
			//		Namespace = x.GetUsingDirective()
			//	}).ToList();
			case FileType.Blazor:
				return fileContent.Where(x => x.StartsWith("@using"))
					.Select(x => new UsingDirectiveInfo
					{
						Namespace = x.GetUsingDirective()
					}).ToList();
			case FileType.CsHtml:
				return fileContent.Where(x => x.StartsWith("@using"))
					.Select(x => new UsingDirectiveInfo
					{
						Namespace = x.GetUsingDirective()
					}).ToList();
			default:
				throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
		}
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

	private IList<TypeInfo> GetTypeInfos(string projectName)
	{
		var directory = _projectFileProvider.GetProjectDirectory(projectName);

		if (directory is null)
		{
			return new List<TypeInfo>();
		}

		var typeFiles = new List<TypeInfo>();
		var extensions = new List<string>
		{
			"*.cs",
			"*.cshtml",
			"*.xaml",
			"*.razor"
		};

		foreach (var extension in extensions)
		{
			var files = Directory.GetFiles(directory, extension, SearchOption.AllDirectories);
			typeFiles.AddRange(files.Select(x =>
			{
				var fileType = ParseFileType(extension);
				return new TypeInfo
				{
					Name = x.GetClassName(),
					FileName = x,
					FileType = fileType,
					UsingDirectives = GetUsingDirectivesForType(x, fileType)
				};
			}));
		}

		return typeFiles;
	}

	private FileType ParseFileType(string fileExtension)
	{
		switch (fileExtension)
		{
			case "*.cs":
				return FileType.CSharp;
			case "*.cshtml":
				return FileType.CsHtml;
			case "*.xaml":
				return FileType.Xaml;
			case "*.razor":
				return FileType.Blazor;
			default:
				return FileType.CSharp;
		}
	}
}