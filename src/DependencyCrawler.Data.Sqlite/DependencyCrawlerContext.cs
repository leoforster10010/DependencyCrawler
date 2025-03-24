using Microsoft.EntityFrameworkCore;

namespace DependencyCrawler.Data.Sqlite;

internal class DependencyCrawlerContext(DbContextOptions options) : DbContext(options)
{
	public DbSet<SerializedDataCore> SerializedDataCores { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<SerializedDataCore>()
			.HasKey(x => x.Id);
		modelBuilder.Entity<SerializedDataCore>()
			.Property(x => x.Id)
			.IsRequired();
		modelBuilder.Entity<SerializedDataCore>()
			.Property(x => x.Payload)
			.IsRequired();
	}
}