using System.ComponentModel.DataAnnotations.Schema;

namespace Guths.Shared.Core.Domain.Interfaces;

public interface IAuditable
{
    [Column(Order = 98)]
    public DateTime CreatedAt { get; set; }

    [Column(Order = 99)]
    public DateTime UpdatedAt { get; set; }

    Guid? UserInclusionId { get; set; }

    Guid? UserChangeId { get; set; }
}
