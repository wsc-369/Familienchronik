using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
  public abstract class DomainModel
  {
    public Guid Id { get; set; }
    public DateTime AddTimestamp { get; set; }
    public DateTime UpdateTimestamp { get; set; }

    public bool Active { get; set; }

    protected DomainModel()
    {
      AddTimestamp = DateTime.Now; // lokale Zeit inkl. Sommerzeit
      UpdateTimestamp = DateTime.Now; // lokale Zeit inkl. Sommerzeit
    }
  }
}
