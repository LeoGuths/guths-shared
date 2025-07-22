namespace Guths.Shared.Helpers;

public static class RouteVersionHelper
{
    public static string BuildRoute(string version, string? route) =>
        $"{version}/{(route ?? string.Empty).TrimStart('/')}";
}
