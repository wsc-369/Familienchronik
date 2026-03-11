using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
  public class GenderStatusOfPerson : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }   // Primärschlüssel
    public GenderStatus Status { get; set; }

    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }

    public enum GenderStatus
    {
      Male = 1, // Männlich 
      Female = 2, // Weiblich 
      Divers = 3, // Für Personen, die sich nicht als männlich oder weiblich identifizieren
      Intersex = 4, // für Personen, deren Geschlechtschromosomen oder anatomische Merkmale nicht dem traditionellen Geschlechtermodell entsprechen
    }
  }
}
