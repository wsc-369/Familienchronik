using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace appAhnenforschungData.DataManager
{
  public class CGlobal
  {
    // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]


    public const Int32 DBTRUE = -1;
    public const Int32 DBFALSE = 0;

    public const string ROLES = "Roles";
    public const string FILTER = "Filter";
    public const string COMPARE = "Compare";
    public const string UPLOAD = "Upload";
    public const string UPLOAD_IMAGEDS = "UploadImages";
    public const string CURRENT_PERSON = "CurrentPerson";
    public const string CURRENT_APPUSER = "CurrentAppUser";
    public const string CURRENT_COMPARES = "CurrentCompares";
    // public const string CURRENT_FAMILIE = "CurrentFamilie";
    public const string ADRESSES = "Adresses";
    public const string ADRESSES_FILTER = "AdressesFilter";
    public const string NICKNAME = "Nickname";

    public const string DEFAULT_PASSWORT = "Ahnenforschung";


    public enum EChronikPages { HISTORY = 1000, HISTORY_MOBILE = 1001, HOME = 2000, HOME_MOBILE = 2001, CONTACT = 3000, CONTACT_MOBILE = 3001, };
    public enum EManualPages { INFO = 500, NEWS_LETTER = 1000, INVITATION = 1100 };
    public enum eDocType { Jahresbericht = 0, Jahresrechnung = 1, Mitgliederversammlung = 2, Budgets = 3 }
    public enum ERole { Mitglied = 1, EditAdress = 2, EditMainPage = 3, Admin = 4 }


    public sealed class ControlChars
    {
      public const char Back = '\b';
      public const char Cr = '\r';
      public const string CrLf = "\r\n";
      public const char FormFeed = '\f';
      public const char Lf = '\n';
      public const string NewLine = "\r\n";
      public const char NullChar = '\0';
      public const char Quote = '"';
      public const char Tab = '\t';
      public const char VerticalTab = '\v';
    }


    public static bool IsLeapYear(DateTime dt)
    {
      bool mod4 = dt.Year % 4 == 0;
      bool mod100 = dt.Year % 100 == 0;
      bool mod400 = dt.Year % 400 == 0;

      return (mod4 && (!mod100 || mod400));
    }


    public static String SplitPersonID(string i_strPersonID)
    {
      string strResult = i_strPersonID;
      if (i_strPersonID != null && i_strPersonID.Length > 0)
      {
        strResult = i_strPersonID.Replace("I", "");
        strResult = strResult.Replace("@", "");
        strResult = strResult.Replace("@", "");
      }

      return strResult;
    }

    public static string bb(string[] i_Languages)
    {
      return "";

    }

    /// <summary>
    /// Serverpfad Applikation
    /// </summary>
    /// <returns></returns>
    public static string GetApplicationPhysicalRootPath()
    {
      return ApplicationPhysicalRootPath();
    }


    /// <summary>
    /// Serverpfad der Bilder 
    /// </summary>
    /// <returns>z.B. c:\</returns>
    public static string GetPhysicalImagePath()
    {
      return Path.Combine(ApplicationPhysicalRootPath(), DirectoryImagePhysicalPath());
    }

    /// <summary>
    /// Server Pfad für den Upload
    /// </summary>
    /// <returns>z.B. c:\</returns>
    public static string GetPhysicalUplaodPath()
    {
      return Path.Combine(ApplicationPhysicalRootPath(), DirectoryFileUploadPhysicalPathRelative());
    }


    /// <summary>
    /// Html Bilder Pfad
    /// </summary>
    /// <returns>z.B. ~/Images/PE</returns>
    public static string GetUrlImagePath()
    {
      return ImageURLPath();
    }


    /// <summary>
    /// Html PDF Pfad
    /// </summary>
    /// <returns>z.B. ~/UploadFiles</returns>
    public static string GetUrlUplaodPath()
    {
      return FileUploadURLPath();
    }


    public static string GetUrlApplication()
    {
      return UrlApplication();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static string ApplicationPhysicalRootPath()
    {
      //return System.Configuration.ConfigurationManager.AppSettings["APPLICATION_PHISICAL_ROOT_PATH_ABSOLUT"];
      return AppDomain.CurrentDomain.BaseDirectory;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>z.B. \UploadFiles</returns>
    public static string DirectoryFileUploadPhysicalPathRelative()
    {
      return ""; // System.Configuration.ConfigurationManager.AppSettings["DIRECTORY_FILE_UPLOAD_PHISICAL_PATH_RELATIV"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>z.B. Images\PE</returns>
    private static string DirectoryImagePhysicalPath()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["DIRECTORY_IMAGE_PHISICAL_PATH_RELATIV"];

    }

    /// <summary>
    /// Verzeichnis der Personenbilder
    /// </summary>
    /// <returns>z.B. ~/Images/PE</returns>
    private static string ImageURLPath()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["IMAGES_URL_PATH"];
    }

    /// <summary>
    /// Verzeichnis der Dokumente wie PDF's
    /// </summary>
    /// <returns>z.B. ~/UploadFiles</returns>
    private static string FileUploadURLPath()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_URL_PATH"];
    }

    /// <summary>
    /// Url auf die Applikation
    /// </summary>
    /// <returns>z.B. http://www.wsc.li/app_ahnenforschung/</returns>
    private static string UrlApplication()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["URL_APPLICATION"];
    }




    public static string EMAIL_CLIENT()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["SMTP_CLIENT"];
    }

    public static string EMAIL_NETWORK_CREDENTIAL_USER()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["NETWORK_CREDENTIAL_USER"];
    }

    public static string EMAIL_NETWORK_CREDENTIAL_PW()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["NETWORK_CREDENTIAL_PW"];
    }


    public static string MAILFORM()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["MAILFORM"];
    }


    public static string EMAIL_MailingList()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["MAILINGLIST"];
    }

    public static bool EMAIL_DO_SEMDMAIL_ADMIN()
    {
      bool bSendMailToAdmin = false;
      //if (System.Configuration.ConfigurationManager.AppSettings["DO_SEMDMAIL_ADMIN"] == "true")
      //{
      //  bSendMailToAdmin = true;
      //}
      return bSendMailToAdmin;
    }

    public static string EMAIL_ADMIN()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["EMAIL_ADMIN"];
    }

    public static string FTP_URL_ROOT()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["FTP_URL_ROOT"];
    }

    public static string FTP_USER()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["FTP_USER"];
    }

    public static string FTP_USER_PW()
    {
      return ""; //System.Configuration.ConfigurationManager.AppSettings["FTP_USER_PW"];
    }

    public static bool DEBUG_MODE()
    {
      bool bDebugMode = true;
      //if (System.Configuration.ConfigurationManager.AppSettings["DEBUG_MODE"] == "true")
      //{
      //  bDebugMode = true;
      //}
      //else
      //{
      //  bDebugMode = false;
      //}
      return bDebugMode;
    }


    public static string CreateRandomPassword()
    {
      int passwordLength = 6;
      //string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
      string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789-";
      char[] chars = new char[passwordLength];
      Random rd = new Random();

      for (int i = 0; i < passwordLength; i++)
      {
        chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
      }

      return new string(chars);
    }


    /// <summary>
    /// Convert null to empty string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertNullToString(object value)
    {
      string result = "";
      if (value == null)
      {
        result = string.Empty;
      }
      else
      {
        result = value.ToString();

      }
      return result;
    }

    public static bool CheckEmail(string i_strEmail)
    {

      string strRegex = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
     + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

      //Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
      Regex regex = new Regex(strRegex);
      Match match = regex.Match(i_strEmail);
      if (match.Success)
      {
        return true;
      }
      else
      {
        return false;
      }

    }

    public CSettings GetSettings()
    {
      CSettings oSettings = new CSettings();
      oSettings.ApplicationPhysicalRootPath = AppDomain.CurrentDomain.BaseDirectory;// CGlobal.GetApplicationPhysicalRootPath();
      oSettings.PhysicalImagePath = GetPhysicalImagePath();
      oSettings.PhysicalUplaodPath = GetPhysicalUplaodPath();
      oSettings.UrlImagePath = GetUrlImagePath();
      oSettings.UrlUplaodPath = GetUrlUplaodPath();
      oSettings.UrlApplication = GetUrlApplication();

      return oSettings;

    }

    public static KeyValuePair<int, int> ImageLargeSize()
    {
            KeyValuePair<int, int> imageSize = new KeyValuePair<int, int>(1920, 1280);
      return imageSize;
    }
    public static KeyValuePair<int, int> ImageSmallSize()
    {
            KeyValuePair<int, int> imageSize = new KeyValuePair<int, int>(1024, 768);
      return imageSize;
    }
    public static KeyValuePair<int, int> ImageThumbSize()
    {
            KeyValuePair<int, int> imageSize = new KeyValuePair<int, int>(600, 400);
      return imageSize;
    }

    public static KeyValuePair<int, int> ImagePersonLargeSize()
    {
            KeyValuePair<int, int> imageSize = new KeyValuePair<int, int>(160, 200);
      return imageSize;
    }
    public static KeyValuePair<int, int> ImagePersonSmallSize()
    {
            KeyValuePair<int, int> imageSize = new KeyValuePair<int, int>(80, 100);
      return imageSize;
    }
    public static KeyValuePair<int, int>ImagePersonThumbSize()
    {
      KeyValuePair<int, int> imageSize = new KeyValuePair<int, int>(46, 60);
      return imageSize;
    }


        public static KeyValuePair<int, int> ImageThumbSizePortal()
        {
            KeyValuePair<int, int> imageSize = new KeyValuePair<int, int>(400, 600);
            return imageSize;
        }

        /// <summary>
        /// Convet Ticks Int to Datetime
        /// </summary>
        /// <param name="lticks"></param>
        /// <returns></returns>
        public static DateTime ConvertTicksToDateTime(Int64 dateticks)
    {
      DateTime dtresult = new DateTime(dateticks);
      return dtresult;
    }


    /// <summary>
    /// Default Datum 
    /// </summary>
    /// <returns></returns>
    public static DateTime CliensSideEmptyDate()
    {
      return new DateTime(1000, 1, 1);
    }

    /// <summary>
    /// Das Alter der Person berechnen
    /// </summary>
    /// <param name="tikBirth"></param>
    /// <param name="tikDeath"></param>
    /// <param name="IsDeath"></param>
    /// <returns></returns>
    public static int CalcualteOlder(UInt64 tikBirth, UInt64 tikDeath, bool IsDeath)
    {

      int value = 0;
      if (tikBirth > 0 && IsDeath)
      {
        TimeSpan ts = ConvertTicksToDateTime(Convert.ToInt64(tikDeath)) - ConvertTicksToDateTime(Convert.ToInt64(tikBirth));
        value = (int)ts.TotalDays / 365;
      }
      else if (tikBirth > 0 && !IsDeath)
      {
        DateTime dtBirth;
        value = 0;
        if (ConvertTicksToDateTime(Convert.ToInt64(tikBirth)).Year > 1)
        {
          int nMonth = ConvertTicksToDateTime(Convert.ToInt64(tikBirth)).Month;
          int nDay = ConvertTicksToDateTime(Convert.ToInt64(tikBirth)).Day;
          if (IsLeapYear(ConvertTicksToDateTime(Convert.ToInt64(tikBirth))) && nMonth == 2 && nDay == 29)
          {
            dtBirth = new DateTime(DateTime.Now.Year, nMonth, nDay - 1);
          }
          else
          {
            dtBirth = new DateTime(DateTime.Now.Year, nMonth, nDay);
          }
          //ts = DateTime.Now - oPerson.Birth;
          value = DateTime.Now.Year - ConvertTicksToDateTime(Convert.ToInt64(tikBirth)).Year;

          if (dtBirth > DateTime.Now)
          {
            value -= 1;

          }
        }
      }
      return value;

    }
  }
}