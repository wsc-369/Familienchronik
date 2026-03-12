using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()] 
  public class CUser
  {
    public int UserId { get; set; }
    public string PersonId { get; set; }
    public string Salutation { get; set; }
    public string Letter { get; set; }
    public string FirstName { get; set; }
    public string PreName { get; set; }
    public string Adress { get; set; }
    public int Zip { get; set; }
    public string Town { get; set; }
    public string Country { get; set; }
    public string Email { get; set; }
    public string Tel { get; set; }
    public string Remarks { get; set; }
    public int Role { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public string AdmissionDateDispaly { get; set; }
    public string CheckOutDateDispaly { get; set; }
    public string LoginName { get; set; }
    public string Password { get; set; }
    public bool? HasPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public string PersonAccessList { get; set; }
    public bool Active { get; set; }
    public bool MustNotPaid { get; set; }
  }
}
