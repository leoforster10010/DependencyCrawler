using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.Implementations.Data.Enum;
using Microsoft.Extensions.Configuration;

namespace DependencyCrawler.Implementations.Repositories.Provider;

internal class ProjectFileProvider : IProjectFileProvider
{
    private const string Extension = "*.csproj";
    private readonly IConfiguration _configuration;

    public ProjectFileProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<string> GetProjectFiles()
    {
        var path = _configuration[ConfigurationKeys.RootDirectory.ToString()];
        return Directory.GetFiles(path!, Extension, SearchOption.AllDirectories).ToList();
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