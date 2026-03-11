using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entity
{
  public class PersonDetail : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string DetailClean { get; set; } = string.Empty;
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
