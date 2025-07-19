using System.ComponentModel;

using Guths.Shared.Core.Extensions;

namespace Guths.Shared.Test.Unit.Core.Extensions;

public sealed class EnumsExtensionsTests
{
    [Fact]
    public void GetDescription_ShouldReturnDescriptionAttribute_WhenExists()
    {
        var result = Status.Pending.GetDescription();
        Assert.Equal("Pending Approval", result);
    }

    [Fact]
    public void GetDescription_ShouldReturnEnumName_WhenNoDescriptionAttribute()
    {
        var result = Status.Rejected.GetDescription();
        Assert.Equal("Rejected", result);
    }

    [Fact]
    public void EnumToSelectList_ShouldReturnAllEnumValues()
    {
        var list = Status.Pending.EnumToSelectList().ToList();

        Assert.Contains(Status.Pending, list);
        Assert.Contains(Status.Approved, list);
        Assert.Contains(Status.Rejected, list);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void GetDescriptionValueList_ShouldReturnCorrectTextAndValuePairs()
    {
        var result = Status.Pending.GetDescriptionValueList().ToList();

        Assert.Contains(result, r => r is { text: "Pending Approval", value: 1 });
        Assert.Contains(result, r => r is { text: "Approved", value: 2 });
        Assert.Contains(result, r => r is { text: "Rejected", value: 3 });
    }

    [Fact]
    public void GetTextList_ShouldReturnEnumNamesAsStrings()
    {
        var result = Status.Pending.GetTextList().ToList();

        Assert.Equal(_expected, result);
    }

    private static readonly string[] _expected = ["Pending", "Approved", "Rejected"];
}

internal enum Status
{
    [Description("Pending Approval")] Pending = 1,
    [Description("Approved")] Approved = 2,
    Rejected = 3
}
