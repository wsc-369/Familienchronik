using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Entity
{
  public class PersonObituary : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string Content { get; set; }
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
