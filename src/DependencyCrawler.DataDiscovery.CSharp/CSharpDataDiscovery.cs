using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.DataDiscovery.CSharp;

internal class CSharpDataDiscovery(IDataCoreProvider dataCoreProvider, IDataCoreDTOFactory dataCoreDTOFactory) : IDataDiscovery
{
	public Guid Id { get; } = Guid.NewGuid();

	public string Name => nameof(CSharpDataDiscovery);

	public async Task Load(string? filePath = null)
	{
		dataCoreProvider.GetOrCreateDataCore(dataCoreDTOFactory.CreateDataCoreDTO(filePath)).Activate();
	}
}