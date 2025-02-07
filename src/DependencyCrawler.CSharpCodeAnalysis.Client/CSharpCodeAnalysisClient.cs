using System.Net.Http.Json;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.CSharpCodeAnalysis.Client;

internal class CSharpCodeAnalysisClient(IDataCoreProvider dataCoreProvider, HttpClient httpClient) : ICSharpCodeAnalysisClient
{
	public async Task GetDataCoreDTOAsync()
	{
		var response = await httpClient.GetAsync("api/CSharpCodeAnalysis");

		if (!response.IsSuccessStatusCode)
		{
			// Log or handle the error appropriately
			return;
		}

		var dataCoreDTO = await response.Content.ReadFromJsonAsync<DataCoreDTO>();

		if (dataCoreDTO is null)
		{
			return;
		}

		dataCoreProvider.GetOrCreateDataCore(dataCoreDTO).Activate();
	}
}