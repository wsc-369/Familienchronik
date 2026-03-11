using appAhnenforschungBackEnd.Configutation;
using appAhnenforschungBackEnd.DataManager;
using appAhnenforschungData.Models.App;
using System;
using System.Text;

namespace appAhnenforschungBackEnd.Comunication
{
  public class CSendMailHelper
  {
    /// <summary>
    /// Passwort anfordern
    /// </summary>
    /// <param name="oUser"></param>
    /// <returns>Es wird ein Standard Passwort verschickt</returns>
    public Boolean SendMailByForgotPassword(CUser oUser)
    {

      // SendMail
      CLSendMail oSendMail = new CLSendMail();
      CMailConfiguration oConfig = new CMailConfiguration();
      oConfig.Client = ReadSettings.EMAIL_CLIENT();
      oConfig.NETWORK_CREDENTIAL_USER = ReadSettings.NETWORK_CREDENTIAL_USER();
      oConfig.NETWORK_CREDENTIAL_PW = ReadSettings.NETWORK_CREDENTIAL_PW();


      // ControllerContext.HttpContext.Session[CGlobal.UPLOAD] = null;

      string strEamilListTo = ReadSettings.EMAIL_MailingList();
      if (oUser.Email != null)
      {
        if (oUser.Email.Length > 0)
        {
          strEamilListTo += oUser.Email;
        }
      }

      return (oSendMail.SendMail(oConfig, ReadSettings.MAILFORM(), strEamilListTo, "Ahnenforschung & Familienchronik", CreateEmailMessage(oUser), false));

    }

    private string CreateEmailMessage(CUser i_oUser)
    {
      string strBody = "Hallo " + i_oUser.PreName + "\n\n"; //\nDeine neuen Zugangsdaten \nBenutzer: " + i_oUser.strLoginName + "\nPasswort: " + i_oUser.strPassword;
      strBody += "Du hast bei der Ahnenforschung ein neues Passwort angefordert.\n\n";
      strBody += "Deine neuen Zugangsdaten sind:\n";
      strBody += "Benutzername: " + i_oUser.LoginName + "\nPasswort: " + i_oUser.Password + "\n";
      strBody += "Dieses Passwort ist nur kurze Zeit gültig, ändere dieses bei der nächsten Anmeldung.\n\n";


      strBody += "Wenn du dich mit diesem eMail nicht angesprochen fühlst, entschuldige bitte den Fehler.\n\n";
      strBody += "Besten Dank.\n Verein\nAhnenforschung und Familienchronik Triesenberg";

      return strBody;

    }

    public string CreatePassword(int length)
    {
      const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
      StringBuilder res = new StringBuilder();
      Random rnd = new Random();
      while (0 < length--)
      {
        res.Append(valid[rnd.Next(valid.Length)]);
      }
      return res.ToString();
    }
  }
}
