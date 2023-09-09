using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data;
using DependencyCrawler.Implementations.Data.Enum;
using Microsoft.Extensions.Configuration;

namespace DependencyCrawler.Implementations.Repositories.Framework;

internal class ConfigurationValidator : IConfigurationValidator
{
    private readonly IConfiguration _configuration;

    public ConfigurationValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsConfigurationValid()
    {
        return RequiredConfigurations.Entries.All(IsConfigurationEntryValid);
    }

    public IReadOnlyList<ConfigurationKeys> GetInvalidConfigurations()
    {
        var invalidEntries = new List<ConfigurationKeys>();

        foreach (var requiredConfiguration in RequiredConfigurations.Entries)
        {
            if (!IsConfigurationEntryValid(requiredConfiguration))
            {
                invalidEntries.Add(requiredConfiguration.Key);
            }
        }

        return invalidEntries;
    }

    private bool IsConfigurationEntryValid(KeyValuePair<ConfigurationKeys, ConfigurationTypes> configurationEntry)
    {
        switch (configurationEntry.Value)
        {
            case ConfigurationTypes.Path:
                return IsPathValid(configurationEntry.Key);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool IsPathValid(ConfigurationKeys pathEntryKey)
    {
        var path = _configuration[pathEntryKey.ToString()];

        return Directory.Exists(path);
    }
}