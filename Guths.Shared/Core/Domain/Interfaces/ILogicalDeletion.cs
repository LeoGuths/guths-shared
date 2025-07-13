namespace Guths.Shared.Core.Domain.Interfaces;

public interface ILogicalDeletion
{
    public DateTime? DeletionDate { get; }

    void MarkAsDeactivated();
    void MarkAsActive();
}
