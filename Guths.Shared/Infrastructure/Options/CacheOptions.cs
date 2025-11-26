using Guths.Shared.Core.Constants;

namespace Guths.Shared.Infrastructure.Options;

public sealed record CacheOptions
{
    public bool UseCache { get; set; } = true;
    public int AbsoluteExpirationInSeconds { get; set; } = Const.TimeAndDate.HourInSeconds;
    public int SlidingExpirationInSeconds { get; set; } = Const.TimeAndDate.MinuteInSeconds * 5;
}
