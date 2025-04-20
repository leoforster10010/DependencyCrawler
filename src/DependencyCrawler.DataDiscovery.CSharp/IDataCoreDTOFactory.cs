using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataDiscovery.CSharp;

public interface IDataCoreDTOFactory
{
	DataCoreDTO CreateDataCoreDTO(string? filePath = null);
}