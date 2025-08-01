using Microsoft.Extensions.Configuration;

namespace Guths.Shared.Core.Extensions;

public static class ConfigurationExtensions
{
    public static string GetRequired(this IConfiguration config, string key)
    {
        var value = config[key];
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Missing required configuration key: {key}");
        return value;
    }
}
