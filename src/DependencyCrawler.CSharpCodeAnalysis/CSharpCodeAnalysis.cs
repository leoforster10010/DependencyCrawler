using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.CSharpCodeAnalysis;

internal class CSharpCodeAnalysis(IDataCoreProvider dataCoreProvider, IDataCoreDTOFactory dataCoreDTOFactory) : ICodeAnalysis
{
	public async Task Load(string? filePath = null)
	{
		dataCoreProvider.GetOrCreateDataCore(dataCoreDTOFactory.CreateDataCoreDTO(filePath)).Activate();
	}
}