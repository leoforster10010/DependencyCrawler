using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IConfigurationValidator
{
    bool IsConfigurationValid();
    IReadOnlyList<ConfigurationKeys> GetInvalidConfigurations();
}