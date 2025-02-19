using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.CSharpCodeAnalysis;

public interface IDataCoreDTOFactory
{
	DataCoreDTO CreateDataCoreDTO(string? filePath = null);
}