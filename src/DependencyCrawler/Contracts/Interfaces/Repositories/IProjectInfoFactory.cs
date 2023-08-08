using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface IProjectInfoFactory
{
	ExternalProjectInfo GetExternalProjectInfo(string dllFilePath);

	InternalProjectInfo GetInternalProjectInfo(string csprojFilePath);
}