using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataDiscovery.CSharp.RESTClient;

internal class GetDataCoreDTOClient(HttpClient httpClient)
{
    public async Task<DataCoreDTO?> GetDataCoreDTO(string? filePath = null)
    {
        var url = "api/DataCoreDTO";
        if (!string.IsNullOrEmpty(filePath))
        {
            url += $"?filePath={Uri.EscapeDataString(filePath)}";
        }

        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            // Log or handle the error appropriately
            return null;
        }

        var contentString = await response.Content.ReadAsStringAsync();

        return DataCoreDTO.Deserialize(contentString);
    }
}