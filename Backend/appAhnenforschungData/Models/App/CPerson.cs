using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()]
  public class CPerson
  {

    public int ID { get; set; }
    [Display(Name = "Ident-Nr.")]
    public string PersonID { get; set; }

    public string FirstName { get; set; }

    public string FamilyName { get; set; }

    [Display(Name = "Name Vorname")]
    public string Fullname { get; set; }
    [Display(Name = "Einbürgerung")]
    public string Bur { get; set; }
    [Display(Name = "Geschlecht")]
    public string Sex { get; set; }
    [Display(Name = "Vater Ident-Nr.")]
    public string FatherID { get; set; }
    [Display(Name = "Mutter Ident-Nr.")]
    public string MotherID { get; set; }
    [Display(Name = "Geburtsort")]
    public string BirthPlace { get; set; }
    [Display(Name = "Sterbeort")]
    public string DeathPlace { get; set; }
    [Display(Name = "Einbürgerungsort")]
    public string BurPlace { get; set; }
    [Display(Name = "Stamm")]
    public string Race { get; set; }
    [Display(Name = "Beruf")]
    public string Work { get; set; }
    [Display(Name = "Name nach Heirat")]
    public string NameMerges { get; set; }
    [Display(Name = "Rufname")]
    public string Nickname { get; set; }
    public string ImagePath { get; set; }
    public string ImagePathSmall { get; set; }
    public string ImagePathBig { get; set; }
    public string ImageName { get; set; }
    //public string LiveDate { get; set; }

    public UInt64 tikBirth { get; set; }
    public UInt64 tikDeath { get; set; }
    public UInt64 tikBur { get; set; }

    [Display(Name = "Geburtsdatum")]
    public DateTime BirthDate { get; set; }
    [Display(Name = "Sterbedatum")]
    public DateTime DeathDate { get; set; }
    [Display(Name = "Einbürgerungsdatum")]
    public DateTime BurDate { get; set; }

    public bool IsDeath { get; set; }
    public bool HasParents { get; set; }
    public bool HasSpouse { get; set; }
    public bool HasChildrens { get; set; }
    [Display(Name = "Aktiv")]
    public bool Active { get; set; }

    [Display(Name = "Alter")]
    public int Older { get; set; }


    //public int Older
    //{
    //  get
    //  {
    //    int value = 0;
    //    if (tikBirth > 0 && IsDeath)
    //    {
    //      TimeSpan ts = ConvertTicksToDateTime(Convert.ToInt64(tikDeath)) - ConvertTicksToDateTime(Convert.ToInt64(tikBirth));
    //      value = (int)ts.TotalDays / 365;
    //    }
    //    else if (tikBirth > 0 && !IsDeath)
    //    {
    //      DateTime dtBirth;
    //      value = 0;
    //      if (ConvertTicksToDateTime(Convert.ToInt64(tikBirth)).Year > 1)
    //      {
    //        int nMonth = ConvertTicksToDateTime(Convert.ToInt64(tikBirth)).Month;
    //        int nDay = ConvertTicksToDateTime(Convert.ToInt64(tikBirth)).Day;
    //        if (IsLeapYear(ConvertTicksToDateTime(Convert.ToInt64(tikBirth))) && nMonth == 2 && nDay == 29)
    //        {
    //          dtBirth = new DateTime(DateTime.Now.Year, nMonth, nDay - 1);
    //        }
    //        else
    //        {
    //          dtBirth = new DateTime(DateTime.Now.Year, nMonth, nDay);
    //        }
    //        //ts = DateTime.Now - oPerson.Birth;
    //        value = DateTime.Now.Year - ConvertTicksToDateTime(Convert.ToInt64(tikBirth)).Year;

    //        if (dtBirth > DateTime.Now)
    //        {
    //          value -= 1;

    //        }
    //      }
    //    }
    //    return value;
    //  }
    //}
    [Display(Name = "Geburtsdatum")]
    public string BirthDisplay { get; set; }
    [Display(Name = "Sterbedatum")]
    public string DeathDisplay { get; set; }
    [Display(Name = "Einbürgerungsdatum")]
    public string BurDisplay { get; set; }
    [Display(Name = "Einbürgerungsdatum")]
    public string Address { get; set; }
    public string FullAdress { get; set; }


    [Display(Name = "Geschlecht")]
    public string SexDisplay { get; set; }

    public List<CPerson> Childs { get; set; }
    public List<CPartner> Partners { get; set; }
    /// <summary>
    /// Das Alter berechnen
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    //private bool IsLeapYear(DateTime dt)
    //{
    //  bool mod4 = dt.Year % 4 == 0;
    //  bool mod100 = dt.Year % 100 == 0;
    //  bool mod400 = dt.Year % 400 == 0;

    //  return (mod4 && (!mod100 || mod400));
    //}

   
  }
}
