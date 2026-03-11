using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Entity.GenderStatusOfPerson;

namespace Entity
{
  public class Person: DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public string PersonRefId{ get; set; }
    public string FamilyName { get; set; } // Mustermann, Nachname (Last Name)
    public string FirstName { get; set; } // Max = Vorname (First Name) 
    public GenderStatus Status { get; set; } // Geschlecht Hier wird der Enum verwendet
    public Guid? FatherId { get; set; } // "Vater Ident-Nr.
    public Guid? MotherId { get; set; } // "Mutte Ident-Nr.
    public string BirthPlace { get; set; } // Geburtsort
    public string DeathPlace { get; set; } // Sterbeort
    public string BurPlace { get; set; } // Einbürgerungsort
    public string Race { get; set; } // Stamm
    public string Work { get; set; } // Beruf
    public string NameMerges { get; set; } // Name nach Heirat
    public string Nickname { get; set; } // Rufname
    public DateTime BirthDate { get; set; } = DateTime.MinValue; // Geburtsdatum
    public DateTime DeathDate { get; set; } = DateTime.MinValue; // Sterbedatum
    public DateTime BurDate { get; set; } = DateTime.MinValue; // Einbürgerungsdatum
    public int BirtYear { get; set; } // Geburtsjahr
    public int DeathYear { get; set; } // Sterbejahr
    public int BirtMonth { get; set; } // Geburtsjahr
    public int DeathMonth { get; set; } // Sterbejahr
    public int BirtDay{ get; set; } // Geburtsjahr
    public int DeathDay { get; set; } // Sterbejahr
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }

    // Navigationen
   // public ICollection<Partner> Partners { get; set; } = new List<Partner>();
  //  public ICollection<Person> Children { get; set; } = new List<Person>();
  }
}
