using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ValueAccess;
using MongoDB.Driver;

namespace DependencyCrawler.Data.MongoDB;

internal class MongoDbDataSource(IDataCoreProvider dataCoreProvider, IMongoDbClientProvider mongoDbClientProvider) : IDataSource
{
	public Guid Id { get; } = Guid.NewGuid();

	public async Task Save()
	{
		var serializedDataCore = new SerializedDataCore
		{
			Id = dataCoreProvider.ActiveCore.Id,
			Payload = dataCoreProvider.ActiveCore.Serialize()
		};
		await mongoDbClientProvider.SerializedDataCoreCollection.ReplaceOneAsync(Builders<SerializedDataCore>.Filter.Eq(dto => dto.Id, serializedDataCore.Id), serializedDataCore, new ReplaceOptions { IsUpsert = true });
	}

	public async Task Load()
	{
		var dataCoreDtos = (await mongoDbClientProvider.SerializedDataCoreCollection.FindAsync(_ => true)).ToList();
		foreach (var dataCoreDto in dataCoreDtos)
		{
			var dataCoreDTO = DataCoreDTO.Deserialize(dataCoreDto.Payload);

			if (dataCoreDTO is null)
			{
				continue;
			}

			dataCoreProvider.GetOrCreateDataCore(dataCoreDTO);
		}
	}
}