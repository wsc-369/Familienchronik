using appAhnenforschungData.Models.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace appAhnenforschungBackEnd.Models
{
  public class CApplicationSession
  {
    private static CApplicationSession instance;
    //private static List<CFamilie> m_arlFamilie;
    //private static List<CAdress> m_arlAdresses;
    //private static CFamilie m_oCurrentFamilie;

    private static List<CPerson> m_arlChildrens;

    public static CApplicationSession Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new CApplicationSession();
          //m_arlFamilie = new List<CFamilie>();
          //m_arlAdresses = new List<CAdress>();
          m_arlChildrens = new List<CPerson>();
        }
        return instance;
      }
    }

    public static void RemoveInstance()
    {
      if (instance != null)
      {
        instance = null;
      }
    }

    //public List<CFamilie> Famliy
    //{
    //  get
    //  {
    //    return m_arlFamilie;
    //  }
    //}

    //public List<CAdress> Adresses
    //{
    //  get
    //  {
    //    return m_arlAdresses;
    //  }
    //  set { m_arlAdresses = value; }
    //}

    //public CFamilie CurrentFamilie
    //{
    //  get
    //  {
    //    return m_oCurrentFamilie;
    //  }
    //  set { m_oCurrentFamilie = value; }
    //}

    public List<CPerson>  Childrens
    {
      get
      {
        return m_arlChildrens;
      }
      set { m_arlChildrens = value; }
    }

    //public void AddFamilie(CFamilie i_Family)
    //{
    //  m_arlFamilie.Add(i_Family);
    //}

    //public CFamilie GetFamily(string i_strPerson)
    //{
    //  if (m_arlFamilie != null && m_arlFamilie.Count > 0)
    //  {
    //    var oPerson = (Famliy.Find(user => user.CurentPerson.PersonID == i_strPerson));
    //    return oPerson;
    //  }
    //  return null;
    //}

    public void AddChildren(CPerson i_Children)
    {
      m_arlChildrens.Add(i_Children);
    }

    public CPerson GeChildren(string i_strPerson)
    {
      if (m_arlChildrens != null && m_arlChildrens.Count > 0)
      {
        var oPerson = (Childrens.Find(user => user.PersonID == i_strPerson));
        return oPerson;
      }
      return null;
    }
  }
}