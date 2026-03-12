using appAhnenforschungData.Models.App;
using appAhnenforschungData.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appAhnenforschungData.DataManager
{
  public class CLAuthentication
  {

    private wsc_chronikContext db = new wsc_chronikContext();

    public CAuthenticationUser AuthenticationUserByEMail(string email, string password)
    {
      try
      {
        CAuthenticationUser oUser = new CAuthenticationUser();
        TUser tuser = db.TUsers.FirstOrDefault(t => t.StrEmail == email && t.StrPassword == password);
        if (tuser != null)
        {
          MappUserEntityToModel(ref oUser, tuser);
        }
        return oUser;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public IList<CAuthenticationUser> AuthenticationUserByEMail(string email)
    {
      try
      {
        List<CAuthenticationUser> oUsers = new List<CAuthenticationUser>();
        CAuthenticationUser oUser = null;
        foreach (TUser tuser in db.TUsers.Where(t => t.StrEmail == email))
        {
          oUser = new CAuthenticationUser();
          MappUserEntityToModel(ref oUser, tuser);
          oUsers.Add(oUser);
        }
        return oUsers;
      }
      catch (Exception)
      {
        throw;
      }
    }


    public CAuthenticationUser AuthenticationByLogin(string login, string password)
    {
      try
      {
        CAuthenticationUser oUser = new CAuthenticationUser();
        TUser tuser = db.TUsers.FirstOrDefault(t => t.StrLoginName == login && t.StrPassword == password);
        if (tuser != null)
        {
          MappUserEntityToModel(ref oUser, tuser);
        }
        return oUser;
      }
      catch (Exception)
      {
        throw;
      }
    }


    private void MappUserEntityToModel(ref CAuthenticationUser oUser, TUser tuser)
    {
      oUser.UserId = tuser.NUserId;
      oUser.PersonId = CGlobal.ConvertNullToString(tuser.StrPersonId);
      oUser.FirstName = CGlobal.ConvertNullToString(tuser.StrName);
      oUser.PreName = CGlobal.ConvertNullToString(tuser.StrPreName);
      oUser.Email = CGlobal.ConvertNullToString(tuser.StrEmail);
      oUser.Role = tuser.NRole;
      oUser.LoginName = CGlobal.ConvertNullToString(tuser.StrLoginName);
      oUser.Password = CGlobal.ConvertNullToString(tuser.StrPassword);
      oUser.Active = tuser.BActive;
    }

    private void MappUserModelToEntity(CAuthenticationUser oUser, ref TUser tuser)
    {
      tuser.NUserId = oUser.UserId;
      tuser.StrPersonId = oUser.PersonId;
      tuser.StrName = oUser.FirstName;
      tuser.StrPreName = oUser.PreName;
      tuser.StrEmail = oUser.Email;
      tuser.NRole = oUser.Role;
      tuser.StrLoginName = oUser.LoginName;
      tuser.StrPassword = oUser.Password;
      tuser.BActive = oUser.Active;
    }
  }
}
