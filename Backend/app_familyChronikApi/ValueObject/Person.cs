using Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static Entity.GenderStatusOfPerson;

namespace ValueObject
{
  public class Person : ValueObject
  {

    public Person()
    {
    }
 
    public Person(Guid id)
    {
      Id = id;
    }

    public Person(Guid id, string personID, string familyName, string firstName, GenderStatus status,
      string birthPlace, string deathPlace, string burPlace, string race, string work, string mameMerges, string nickname,
      DateTime birthDate, DateTime deathDate, DateTime burDate,
      Person father , Person mother,
      bool active = true) // IEnumerable<Person> personPartners = null, IEnumerable<Person> children = null
    {
   
      if (string.IsNullOrWhiteSpace(personID))
        throw new ArgumentException("Person id is required.");
      //if (string.IsNullOrWhiteSpace(familyname))
      //  throw new ArgumentException("Family name is required.");
      //if (string.IsNullOrWhiteSpace(firstName))
      //  throw new ArgumentException("First name is required.");


      Id = id;
      PersonID = personID;
      FamilyName = familyName;
      FirstName = firstName;
      Sex = status == GenderStatus.Male ? "M": "F" ;
      SexDisplay = status == GenderStatus.Male ? "männlich" : "wieblich";
      BirthPlace = birthPlace;
      DeathPlace = deathPlace;
      BurPlace = burPlace;
      Race = race;
      Work = work;
      NameMerges = mameMerges;
      Nickname = nickname;
      BirthDate = birthDate;
      DeathDate = deathDate;
      BurDate = burDate;
      Father = father;
      Mother = mother;
      Active = active;


      // Partner = partner; // kann null sein
      //  Children = children?.ToList().AsReadOnly() ?? new List<Person>().AsReadOnly();
    }

    public string PersonID { get; set; }

    public Person Father { get; set; }

    public Person Mother { get; set; }

    public string FamilyName { get; set; }

    public string FirstName { get; set; } // Max = Vorname (First Name) 

    public string Fullname => $"{FamilyName} {FirstName}";


    public string Bur { get; set; } // Einbürgerung

    public string Sex { get; set; } // Geschlecht, Hier wird der Enum verwendet

    public string BirthPlace { get; set; }    // Geburtsort

    public string DeathPlace { get; set; } // Sterbeort

    public string BurPlace { get; set; } // Einbürgerungsort

    public string Race { get; set; } // Stamm

    public string Work { get; set; }

    public string NameMerges { get; set; } // Name nach Heirat

    public string Nickname { get; set; } // Rufname

    public DateTime BirthDate { get; set; }

     public DateTime DeathDate { get; set; }

    public DateTime BurDate { get; set; } // Einbürgerungsdatum

    public bool Active { get; set; }


    public void AddPartner(Partner partner)
    {
      Partners.Add(partner);
    }

    public void AddPartnerPerson(Person partnerPerson)
    {
      PersonPartners.Add(partnerPerson);
    }

    public void AddChildren(Person child)
    {
      Childrens.Add(child);
    }

    public IList<Partner> Partners { get; set; }

    public List<Person> PersonPartners { get; set; }

    public List<Person> Childrens { get; set; }

    public List<PersonPortrait> PersonPortraits { get; set; }
    
    public List<PersonImage> PersonImages { get; set; }

    public int Older { get; set; }


    public bool IsMarriage()
    {
      return Partners != null && Partners.FirstOrDefault().MarriageDateTime != DateTime.MinValue;
    }

    public bool IsDivorce()
    {
      return Partners != null && Partners.FirstOrDefault().DivorceDateTime != DateTime.MinValue;
    }

    public string Address { get; set; }
    public string FullAdress { get; set; }


    [Display(Name = "Geschlecht")]
    public string SexDisplay { get; set; }

  
    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return PersonID;
      yield return FamilyName;
      yield return FirstName;
      yield return Bur;
      yield return Sex;
      yield return BirthPlace;
      yield return DeathPlace;
      yield return BurPlace;
      yield return Father;
      yield return Mother;

      foreach (var partner in Partners)
        yield return partner;
    }
  }
}