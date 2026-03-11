using System;

namespace app_familyBackend.KI
{
  public class PersonData
  {
    public string Name { get; set; }
    public string Vorname { get; set; }
    public string Geburtsdatum { get; set; }
    public string Sterbedatum { get; set; }
    public string Heiratsdatum { get; set; }
    public string PartnerName { get; set; }
    public string PartnerVorname { get; set; }
    public string PartnerGeburtsdatum { get; set; }
    public string PartnerSterbedatum { get; set; }

    public int PersonNr { get; set; }
    public int PartnerNr { get; set; }
  }
}
