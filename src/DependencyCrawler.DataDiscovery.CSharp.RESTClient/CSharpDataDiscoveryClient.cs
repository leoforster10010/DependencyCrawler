using System.Net.Http.Json;
using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.DataDiscovery.CSharp.RESTClient;

internal class CSharpDataDiscoveryClient(IDataCoreProvider dataCoreProvider, GetDataCoreDTOClient getDataCoreDtoClient) : IDataDiscovery
{
	public Guid Id { get; } = Guid.NewGuid();

	public string Name => nameof(CSharpDataDiscoveryClient);

	public async Task Load(string? filePath = null)
	{
		var dataCoreDTO = await getDataCoreDtoClient.GetDataCoreDTO(filePath);

		if (dataCoreDTO is null)
		{
			return;
		}

		dataCoreProvider.GetOrCreateDataCore(dataCoreDTO).Activate();
	}
}