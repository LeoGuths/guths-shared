using System.Reflection;

namespace Guths.Shared.Core.Extensions;

public static class AssemblyExtensions
{
    public static string GetEntryAssemblyVersion()
        => Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";
}

