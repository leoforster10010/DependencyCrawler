using System.Net.Http.Json;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.CSharpCodeAnalysis.Client;

internal class CSharpCodeAnalysisClient(IDataCoreProvider dataCoreProvider, HttpClient httpClient) : ICodeAnalysis
{
	public async Task Load(string? filePath = null)
	{
		var url = "api/CSharpCodeAnalysis";
		if (!string.IsNullOrEmpty(filePath))
		{
			url += $"?filePath={Uri.EscapeDataString(filePath)}";
		}

		var response = await httpClient.GetAsync(url);

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