using System.ComponentModel.DataAnnotations.Schema;

using Guths.Shared.Core.Domain.Interfaces;

namespace Guths.Shared.Core.Domain.Entities;

public class BaseEntityLogicalDeletion : BaseEntity, IAuditable, ILogicalDeletion
{
    [Column(Order = 98)] public DateTime CreatedAt { get; set; }
    [Column(Order = 99)] public DateTime UpdatedAt { get; set; }
    public DateTime? DeletionDate { get; private set; }
    public Guid? UserInclusionId { get; set; }
    public Guid? UserChangeId { get; set; }

    public void MarkAsDeactivated() => DeletionDate = DateTime.UtcNow;
    public void MarkAsActive() => DeletionDate = null;
}
