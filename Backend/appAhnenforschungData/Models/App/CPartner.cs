using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()] 
  public class CPartner
  {
    //public string m_strPartnerID;
    //public string m_strMarriageDate;
    //public string m_strDivorceDate;
    //public DateTime m_dtMarriageDateTime = new DateTime(1900, 01, 01);
    //public DateTime m_dtDivorceDateTime = new DateTime(1900, 01, 01);
    public string m_strClassName;

    //public bool m_bIsMarriaged;
    //public bool m_bIsDivorced;
    public bool m_bIsCurrent;

    //public UInt64 tikMarriageDate { get { return tikMarriageDate; } set { tikMarriageDate = value; } }
    //public UInt64 tikDivorceDate { get { return tikDivorceDate; } set { tikDivorceDate = value; } }

    //[Display(Name = "Ident-Nr.")]
    //public string PartnerID { get { return m_strPartnerID; } set { m_strPartnerID = value; } }
    
    //[Display(Name = "Heirat am")]
    //public string MarriageDateDisplay { get { return Convert.ToDateTime(tikMarriageDate).ToShortDateString(); } }

    //[Display(Name = "Geschieden am")]
    //public string DivorceDateDisplay { get { return Convert.ToDateTime(tikDivorceDate).ToShortDateString(); } }

    //[Display(Name = "Verheiratet ja/nein")]
    //public bool IsMarriageDate { get { return tikMarriageDate>0; }}
    //[Display(Name = "Geschieden ja/nein")]
    //public bool IsDivorceDate { get { return tikDivorceDate>0; }}
    //[Display(Name = "Aktuell")]
    //public bool IsCurrent { get { return m_bIsCurrent; } set { m_bIsCurrent = value; } }

    //public string ClassName { get { return m_strClassName; } set { m_strClassName = value; } }

  

    public UInt64 tikMarriageDate { get; set; }
    public UInt64 tikDivorceDate { get; set; }

    [Display(Name = "Ident-Nr.")]
    public string PartnerID { get; set; }

    [Display(Name = "Person ID")]
    public string PersonID { get; set; }

    [Display(Name = "Heirat am")]
    public string MarriageDateDisplay { get; set; }

    [Display(Name = "Geschieden am")]
    public string DivorceDateDisplay { get; set; }

    [Display(Name = "Heirat am")]
    public DateTime MarriageDateTime { get; set; }

    [Display(Name = "Geschieden am")]
    public DateTime DivorceDateTime { get; set; }

    [Display(Name = "Verheiratet ja/nein")]
    public bool IsMarriageDate { get; set; }
    [Display(Name = "Geschieden ja/nein")]
    public bool IsDivorceDate { get; set; }
    [Display(Name = "Aktuell")]
    public bool IsCurrent { get { return m_bIsCurrent; } set { m_bIsCurrent = value; } }

    public string ClassName { get { return m_strClassName; } set { m_strClassName = value; } }
    public CPerson Person { get; set; }
  }
}
