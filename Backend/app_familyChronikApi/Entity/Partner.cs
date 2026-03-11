using System;
using System.ComponentModel.DataAnnotations;
using static Entity.PartnerConnectionRole;

namespace Entity
{
  public class Partner : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }

    public Guid PersonId { get; set; }

    public Guid PartnerId { get; set; }

    public DateTime MarriageDateTime { get; set; }

    public DateTime DivorceDateTime { get; set; }

    public int MarriageYear { get; set; }

    public int DivorceYear { get; set; }

    public int ConnectionRole { get; set; } // Hier wird der Enum verwendet

    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
