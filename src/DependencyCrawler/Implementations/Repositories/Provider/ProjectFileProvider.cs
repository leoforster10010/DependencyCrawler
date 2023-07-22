using DependencyCrawler.Contracts.Interfaces;
using DependencyCrawler.Framework.Extensions;
using Microsoft.Extensions.Configuration;

namespace DependencyCrawler.Implementations.Repositories.Provider;

public class ProjectFileProvider : IProjectFileProvider
{
	private const string Extension = "*.csproj";
	private readonly IEnumerable<string> _projectFiles;

	public ProjectFileProvider(IConfiguration configuration)
	{
		var path = configuration["RootDirectory"];
		_projectFiles = Directory.GetFiles(path, Extension, SearchOption.AllDirectories).ToList();
	}

	public IEnumerable<string> GetProjectFiles()
	{
		return _projectFiles;
	}

	public string? GetProjectFile(string projectName)
	{
		return _projectFiles.FirstOrDefault(x => x.GetProjectName() == projectName);
	}

	public string? GetProjectDirectory(string projectName)
	{
		return Path.GetDirectoryName(GetProjectFile(projectName));
	}

	public bool GetIsInternal(string projectName)
	{
		return _projectFiles.Any(x => x.GetProjectName() == projectName);
	}
}