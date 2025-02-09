using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.CSharpCodeAnalysis.REST;

internal class CSharpCodeAnalysis(IDataCoreProvider dataCoreProvider, IDataCoreDTOFactory dataCoreDTOFactory) : ICodeAnalysis
{
	public async Task Load()
	{
		dataCoreProvider.GetOrCreateDataCore(dataCoreDTOFactory.CreateDataCoreDTO());
	}
}