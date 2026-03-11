using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Entity.GenderStatusOfPerson;

namespace Entity
{
  public class ParentChild : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public new bool Active { get; set; }
    public Person Child { get; set; }
    public Person Parent { get; set; }
    public int ParentRole { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
