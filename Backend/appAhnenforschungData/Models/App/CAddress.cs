using System;
using System.Collections.Generic;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()]
  public class CAddress
  {
    public int AddressId { get; set; }
    public string PersonId { get; set; }
    public string Adresse { get; set; }
    public string HouseNr { get; set; }
    public string Town { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }
    public int? OrderNr { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? CreateCreate { get; set; }
    public bool? Active { get; set; }
  }
}
