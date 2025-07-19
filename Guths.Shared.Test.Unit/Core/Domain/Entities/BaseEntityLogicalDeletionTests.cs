using Guths.Shared.Core.Domain.Entities;

namespace Guths.Shared.Test.Unit.Core.Domain.Entities;

public sealed class BaseEntityLogicalDeletionTests
{
    [Fact]
    public void NewEntity_ShouldHaveNullDeletionDate()
    {
        var entity = new BaseEntityLogicalDeletion();

        Assert.Null(entity.DeletionDate);
    }

    [Fact]
    public void MarkAsDeactivated_ShouldSetDeletionDateToUtcNow()
    {
        var entity = new BaseEntityLogicalDeletion();
        var before = DateTime.UtcNow;

        entity.MarkAsDeactivated();
        var after = DateTime.UtcNow;

        Assert.NotNull(entity.DeletionDate);
        Assert.InRange(entity.DeletionDate.Value, before, after);
    }

    [Fact]
    public void MarkAsActive_ShouldClearDeletionDate()
    {
        var entity = new BaseEntityLogicalDeletion();
        entity.MarkAsDeactivated();
        Assert.NotNull(entity.DeletionDate);

        entity.MarkAsActive();

        Assert.Null(entity.DeletionDate);
    }
}
