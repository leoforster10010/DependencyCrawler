using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.CSharpCodeAnalysis;

internal class CSharpCodeAnalysis(IDataCoreProvider dataCoreProvider, IDataCoreDTOFactory dataCoreDTOFactory) : ICodeAnalysis
{
	public async Task Load()
	{
		dataCoreProvider.GetOrCreateDataCore(dataCoreDTOFactory.CreateDataCoreDTO()).Activate();
	}
}