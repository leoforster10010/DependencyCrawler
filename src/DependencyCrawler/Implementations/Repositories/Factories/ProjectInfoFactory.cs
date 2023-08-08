using System.Reflection;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;
using Microsoft.Build.Construction;

namespace DependencyCrawler.Implementations.Repositories.Factories;

internal class ProjectInfoFactory : IProjectInfoFactory
{
	private readonly IExternalProjectInfoLoader _externalProjectInfoLoader;
	private readonly IInternalProjectInfoLoader _internalProjectInfoLoader;

	public ProjectInfoFactory(IInternalProjectInfoLoader internalProjectInfoLoader,
		IExternalProjectInfoLoader externalProjectInfoLoader)
	{
		_internalProjectInfoLoader = internalProjectInfoLoader;
		_externalProjectInfoLoader = externalProjectInfoLoader;
	}

	public InternalProjectInfo GetInternalProjectInfo(string csprojFilePath)
	{
		var projectRootElement = GetProjectRootElement(csprojFilePath);
		var projectInfo = GetInternalProjectInfo(projectRootElement);

		return projectInfo;
	}

	public ExternalProjectInfo GetExternalProjectInfo(string dllFilePath)
	{
		var assembly = GetAssembly(dllFilePath);
		if (assembly is null)
		{
			return new ExternalProjectInfo
			{
				Name = dllFilePath.GetDllName()
			};
		}

		var externalProjectInfo = GetExternalProjectInfo(assembly);

		return externalProjectInfo;
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

	private InternalProjectInfo GetInternalProjectInfo(ProjectRootElement projectRootElement)
	{
		var name = projectRootElement.FullPath.GetProjectName();
		var packageReferences = projectRootElement.GetPackageReferences().ToList();
		var projectReferences = projectRootElement.GetProjectReferences().ToList();
		var namespaces = _internalProjectInfoLoader.LoadNamespaces(name).ToList();

		return new InternalProjectInfo
		{
			Name = name,
			Namespaces = namespaces,
			PackageReferences = packageReferences,
			ProjectReferences = projectReferences
		};
	}

	private ExternalProjectInfo GetExternalProjectInfo(Assembly assembly)
	{
		var name = assembly.GetProjectName();
		var packageReferences = assembly.GetPackageReferenceInfos().ToList();
		var namespaces = _externalProjectInfoLoader.LoadNamespaces(assembly);

		return new ExternalProjectInfo
		{
			Name = name,
			Namespaces = namespaces,
			PackageReferences = packageReferences
		};
	}
}