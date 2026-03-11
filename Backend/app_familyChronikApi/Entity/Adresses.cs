using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
  public class Address : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string Street { get; set; }
    public string HouseNr { get; set; }
    public string Town { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }
    public string FullAddress { get; set; }
    public int OrderNr { get; set; }
    public string Description { get; set; }
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
