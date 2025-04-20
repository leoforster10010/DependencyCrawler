using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.Data.Postgresql;

internal class PostgresqlDataSource(DependencyCrawlerContext dependencyCrawlerContext, IDataCoreProvider dataCoreProvider) : IDataSource
{
	public Guid Id { get; } = Guid.NewGuid();

	public async Task Save()
	{
		if (dependencyCrawlerContext.SerializedDataCores.Any(x => x.Id == dataCoreProvider.ActiveCore.Id))
		{
			dependencyCrawlerContext.SerializedDataCores.Update(new SerializedDataCore
			{
				Id = dataCoreProvider.ActiveCore.Id,
				Payload = dataCoreProvider.ActiveCore.Serialize()
			});
		}
		else
		{
			dependencyCrawlerContext.SerializedDataCores.Add(new SerializedDataCore
			{
				Id = dataCoreProvider.ActiveCore.Id,
				Payload = dataCoreProvider.ActiveCore.Serialize()
			});
		}

		await dependencyCrawlerContext.SaveChangesAsync();
	}

	public async Task Load()
	{
		foreach (var serializedDataCore in dependencyCrawlerContext.SerializedDataCores)
		{
			var dataCoreDTO = DataCoreDTO.Deserialize(serializedDataCore.Payload);
			if (dataCoreDTO is not null)
			{
				dataCoreProvider.GetOrCreateDataCore(dataCoreDTO);
			}
		}
	}

	public string Name => nameof(PostgresqlDataSource);
}