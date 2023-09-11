using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.Implementations.Data.Enum;
using Microsoft.Extensions.Configuration;

namespace DependencyCrawler.Implementations.Repositories.Provider;

internal class ProjectFileProvider : IProjectFileProvider
{
	private const string Extension = "*.csproj";
	private readonly IEnumerable<string> _projectFiles;

	public ProjectFileProvider(IConfiguration configuration)
	{
		var path = configuration[ConfigurationKeys.RootDirectory.ToString()];
		_projectFiles = Directory.GetFiles(path!, Extension, SearchOption.AllDirectories);
	}

	public IEnumerable<string> GetProjectFiles()
	{
		return _projectFiles;
	}

	public string? GetProjectFile(string projectName)
	{
		return GetProjectFiles().FirstOrDefault(x => x.GetProjectName() == projectName);
	}

	public string? GetProjectDirectory(string projectName)
	{
		return Path.GetDirectoryName(GetProjectFile(projectName));
	}

	public bool GetIsInternal(string projectName)
	{
		return GetProjectFiles().Any(x => x.GetProjectName() == projectName);
	}
}