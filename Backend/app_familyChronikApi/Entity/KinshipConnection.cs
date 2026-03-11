using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
  public class KinshipConnection : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public Guid PersonId { get; set; }
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
