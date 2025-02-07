using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.CSharpCodeAnalysis.REST;

public interface IDataCoreDTOFactory
{
	DataCoreDTO CreateDataCoreDTO();
}