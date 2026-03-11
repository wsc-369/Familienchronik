using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
  public class DialectWord : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Voice { get; set; }
    public int Place { get; set; }
    public int Source { get; set; }
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
