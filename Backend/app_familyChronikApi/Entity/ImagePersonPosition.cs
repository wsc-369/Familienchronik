using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
  public class ImagePersonPosition : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }

    public Guid ImagePersonId { get; set; }

    public Guid PersonId { get; set; }

    public int Pos { get; set; }

    public string FamilyName { get; set; } // Schädler

    public string FirstName { get; set; } // Walter

    public string Address { get; set; }

    public string HouseNo { get; set; }

    public string Zip { get; set; }

    public string Country { get; set; }

    public string Town { get; set; }

    public int BirthYear { get; set; }

    public string ReferencePersonId { get; set; }

    public string Description { get; set; }

    public string EditContactData { get; set; }

    public string EditEmail { get; set; }

    public bool Finish { get; set; }

    public new bool Active { get; set; }
    
    public new DateTime AddTimestamp { get; set; }
    
    public new DateTime UpdateTimestamp { get; set; } 
  }
}
