using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Data;

public static class RequiredConfigurations
{
    public static IReadOnlyDictionary<ConfigurationKeys, ConfigurationTypes> Entries =>
        new Dictionary<ConfigurationKeys, ConfigurationTypes>
        {
            {ConfigurationKeys.RootDirectory, ConfigurationTypes.Path}
        };
}