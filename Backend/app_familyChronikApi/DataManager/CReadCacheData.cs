using appAhnenforschungBackEnd.Models;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using appAhnenforschungData.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace appAhnenforschungBackEnd.DataManager
{
  internal class CReadCacheData
  {
    private wsc_chronikContext db = new wsc_chronikContext();

    public CPerson GetCachedPersonByID(string idPerson, CSettings i_oSettings)
    {
      try
      {
        CPerson oChildren = CApplicationSession.Instance.GeChildren(idPerson);
        if (oChildren == null)
        {
          appAhnenforschungData.DataManager.CReadWriteData oRead = new appAhnenforschungData.DataManager.CReadWriteData();
          CPerson oPerson = new CPerson();

          TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == idPerson);
          if (tperson != null)
          {
            oRead.MappPersonEntityToModelChildrenCache(ref oPerson, tperson, i_oSettings);
            CApplicationSession.Instance.AddChildren(oPerson);
          }
          return oPerson;
        }
        else
        {
          return oChildren;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public List<CPerson> GetCachedChildrenByPersonID(string idPerson, CSettings oSettings)
    {
      try
      {
        List<CPerson> arlchildrens = new List<CPerson>();

        CReadWriteData oRead = new CReadWriteData();

        TPerson operson = db.TPersons.FirstOrDefault(t => t.StrPersonId == idPerson);
        if (operson != null)
        {
          if (operson.StrSex == "M")
          {
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrFatherId == operson.StrPersonId))
            {
              CPerson oPerson = new CPerson();
              oPerson = CApplicationSession.Instance.GeChildren(tperson.StrPersonId);
              if (oPerson == null)
              {
                oPerson = new CPerson();
                oRead.MappPersonEntityToModelChildrenCache(ref oPerson, tperson, oSettings);
                CApplicationSession.Instance.AddChildren(oPerson);
              }
              arlchildrens.Add(oPerson);
            }

          }
          else if (operson.StrSex == "F")
          {
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrMotherId == operson.StrPersonId))
            {
              CPerson oPerson = new CPerson();
              oPerson = CApplicationSession.Instance.GeChildren(tperson.StrPersonId);

              if (oPerson == null)
              {
                oPerson = new CPerson();
                oRead.MappPersonEntityToModelChildrenCache(ref oPerson, tperson, oSettings);
                CApplicationSession.Instance.AddChildren(oPerson);
              }
              arlchildrens.Add(oPerson);
            }
          }
        }
        return arlchildrens;
      }
      catch (Exception)
      {
        throw;
      }
    }
  }
}