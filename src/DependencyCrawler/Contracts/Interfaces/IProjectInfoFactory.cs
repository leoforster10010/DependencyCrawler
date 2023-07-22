using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces;

public interface IProjectInfoFactory
{
	ExternalProjectInfo GetExternalProjectInfo(string dllFilePath);

	InternalProjectInfo GetInternalProjectInfo(string csprojFilePath);
}