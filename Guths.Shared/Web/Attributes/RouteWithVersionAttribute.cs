using Guths.Shared.Web.Versioning;

using Microsoft.AspNetCore.Mvc;

namespace Guths.Shared.Web.Attributes;

public sealed class RouteWithVersionAttribute : RouteAttribute
{
    public RouteWithVersionAttribute(string template, EApiVersions actionVersion = EApiVersions.V1)
        : base($"{GetVersionNameByType(actionVersion)}/{template}")
    {}

    private static string GetVersionNameByType(EApiVersions actionVersion) =>
        actionVersion switch
        {
            EApiVersions.V1 => "v1",
            EApiVersions.V2 => "v2",
            _ => "v1"
        };
}
