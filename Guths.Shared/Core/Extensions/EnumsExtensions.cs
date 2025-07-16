using System.ComponentModel;
using System.Globalization;

namespace Guths.Shared.Core.Extensions;

public static class EnumsExtensions
{
    public static string GetDescription<T>(this T e) where T : IConvertible
    {
        var description = e.ToString();

        if (e is not Enum) return description ?? string.Empty;

        var type = e.GetType();
        var values = Enum.GetValues(type);

        foreach (int val in values)
        {
            if (val != e.ToInt32(CultureInfo.InvariantCulture)) continue;

            var memInfo = type.GetMember(type.GetEnumName(val)!);
            var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descriptionAttributes.Length > 0)
            {
                description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
            }

            break;
        }

        return description ?? string.Empty;
    }

    public static IEnumerable<T> EnumToSelectList<T>(this T _) where T : IConvertible
        => Enum.GetValues(typeof(T)).Cast<T>();

    public static IEnumerable<(string text, int value)> GetDescriptionValueList<T>(this T e)
        where T : IConvertible
        => e.EnumToSelectList().Select(convertible => (convertible.GetDescription(), convertible.ToInt32(CultureInfo.InvariantCulture)));

    public static IEnumerable<string?> GetTextList<T>(this T e)
        where T : IConvertible
        => e.EnumToSelectList().Select(convertible => convertible.ToString());
}
