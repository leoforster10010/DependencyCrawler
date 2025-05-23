using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ValueAccess;
using DependencyCrawler.Framework;
using Microsoft.EntityFrameworkCore;

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
		var serializedDataCores = await dependencyCrawlerContext.SerializedDataCores.Select(x => DataCoreDTO.Deserialize(x.Payload)).NotNull().ToListAsync();
		foreach (var serializedDataCore in serializedDataCores)
		{
			dataCoreProvider.GetOrCreateDataCore(serializedDataCore);
		}
	}

	public string Name => nameof(PostgresqlDataSource);
}