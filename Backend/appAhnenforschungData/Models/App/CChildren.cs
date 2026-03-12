using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()]
  public class CChildren : CPerson
  {
    public int ChildrenID { get ;  set; }
    //[Display(Name = "Kind-Nr.")]
    //public string m_nChildrenID;
    //[Display(Name = "Person-Nr.")]
    //public string m_strPersonID;
    //[Display(Name = "Vater-Nr.")]
    //public string m_strFatherID;
    //[Display(Name = "Mutter-Nr.")]
    //public string m_strMotherID;
    //[Display(Name = "Name, Vorname")]
    //public string m_strFullName;

    
    //public string ImagePath { get; set; }
    //public string ImagePathSmall { get; set; }
    //public string ImagePathBig { get; set; }
    //public string ImageName { get; set; }
    //public string LiveDate { get; set; }
    //public bool IsDeath { get; set; }
  

    //public UInt64 tikBirth{ get { return m_tikBirth; } set { m_tikBirth = value; } }
    //public UInt64 tikDeath{ get { return m_tkDeath; } set { m_tkDeath = value; } }
    //public UInt64 tikBur { get { return m_tkBur; } set { m_tkBur = value; } }

  }
}
