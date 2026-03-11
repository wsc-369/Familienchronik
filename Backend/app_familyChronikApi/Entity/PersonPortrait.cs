using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entity
{
  public class PersonPortrait : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; }= string.Empty;
    public string PdfName { get; set; } = string.Empty;
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
