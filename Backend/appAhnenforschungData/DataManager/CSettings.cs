using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appAhnenforschungData.DataManager
{
  [Serializable()]
  public class CSettings
  {

    public string ApplicationPhysicalRootPath { get; set; }
    public string PhysicalImagePath { get; set; }
    public string PhysicalUplaodPath { get; set; }
    public string UrlImagePath { get; set; }
    public string UrlUplaodPath { get; set; }
    public string UrlApplication { get; set; }

    public string UrlRessources { get; set; }
    

    public string FileNameInvitation
    {
      get { return "Invitation.html"; }
    }

    public  string FileNameNewsLetter
    {
      get { return "NewsLetter.html"; }
    }

    //public string Name { get; set; }
    //public string PreName { get; set; }
    //public string Adress { get; set; }
    //public int Zip { get; set; }
    //public string Town { get; set; }
    //public string Country { get; set; }
    //public string ImagePath { get; set; }
    //public string LoginName { get; set; }
    //public string Password { get; set; }
    //public string Eamil { get; set; }
    //public string Salutation { get; set; }
  }
}
