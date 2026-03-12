using System;
using System.ComponentModel.DataAnnotations;
using static appAhnenforschungBackEnd.DataManager.CGlobal;

namespace ValueObject
{
  public class AppUser : ValueObject
  {
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public Guid PersonId { get; set; }
    public string? Salutation { get; set; }
    public string? Letter { get; set; }
    public string? FamilyName { get; set; }
    public string? FirstName { get; set; }
    public string? Adress { get; set; }
    public int Zip { get; set; }
    public string? Town { get; set; }
    public string? Country { get; set; }
    public string? Email { get; set; }
    public string? Tel { get; set; }
    public string? Remarks { get; set; }
    public AppUserRoles Role { get; set; }= AppUserRoles.None;
    public DateTime AdmissionDate { get; set; } = DateTime.MinValue;
    public DateTime CheckOutDate { get; set; } = DateTime.MinValue;
    public string LoginName { get; set; }
    public string Password { get; set; }
    public bool? HasPaid { get; set; }
    public DateTime PaidDate { get; set; } = DateTime.MinValue;
    public string? PersonAccessList { get; set; }
    public new bool Active { get; set; }
    public bool MustNotPaid { get; set; }
   
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      throw new NotImplementedException();
    }
  }
}
