using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.CSharpCodeAnalysis.REST;

internal class CSharpCodeAnalysis(IDataCoreProvider dataCoreProvider, IDataCoreDTOFactory dataCoreDTOFactory) : ICodeAnalysis
{
	public void Load()
	{
		dataCoreProvider.GetOrCreateDataCore(dataCoreDTOFactory.CreateDataCoreDTO());
	}
}