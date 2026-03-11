using Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
  public class ParentRole : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public int Role { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public new bool Active { get; set; }
   // public new DateTime AddTimestamp { get; set; }
  //  public new DateTime UpdateTimestamp { get; set; }
  }
}