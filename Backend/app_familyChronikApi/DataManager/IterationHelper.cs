using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text;
using appAhnenforschungData.Models.App;

namespace appAhnenforschungBackEnd.DataManager
{
  public static class IterationHelper
  {

    private static IHttpContextAccessor _contextAccessor;

    //public class FakeView : IView
    //{
    //  /// <inheritdoc />
    //  public Task RenderAsync(ViewContext context)
    //  {
    //    return Task.CompletedTask;
    //  }

    //  /// <inheritdoc />
    //  public string Path { get; } = "View";
    //}

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
      _contextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Eine komplette Ahnentafel erstellen
    /// </summary>
    /// <param name="_object">AhnenforschungModelPresentation.CPerson </param>
    /// <returns></returns>
    ///  public static IHtmlContent ShowFamilyTree(string baseUrl, CPerson _object)
    public static IHtmlContent ShowFamilyTree(string baseUrl, CPerson _object)
    {
      // IHtmlHelper htmlHelper;//= Create();

      StringBuilder output = new StringBuilder();

      if (_object.Childs.Count == 0)
      {
        output.Append("<ul>");
        output.Append("<li>");
        BuildParent(baseUrl, _object, output);
        output.Append("</li>");
        output.Append("</ul>");
      }

      else if (_object.Childs.Count > 0)
      {
        output.Append("<ul>");
        output.Append("<li>");
        BuildParent(baseUrl, _object, output);
        output.Append("<ul>");

        // Schleife durch alle Childs vom Parent
        _object.Childs.Sort((x, y) => DateTime.Compare(x.BirthDate, y.BirthDate));
        foreach (CPerson subItem in _object.Childs)
        {
          output.Append("<li>");
          BuildPerson(baseUrl, subItem, output);
          IterationSubChilds(baseUrl, subItem, output);
          output.Append("</li>");
        }
        output.Append("</ul>");

        output.Append("</li>");
        output.Append("</ul>");
      }

      // https://stackoverflow.com/questions/33667308/convert-ihtmlcontent-tagbuilder-to-string-in-c-sharp
      return new HtmlString(output.ToString());
      //return GetString(new HtmlString(output.ToString()));
    }

    /// <summary>
    /// Iteration durch alle Personen
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="output"></param>
    private static void IterationSubChilds(string baseUrl, CPerson _object, StringBuilder output)
    {
      string url = string.Empty;

      if (_object.Childs.Count > 0)
      {
        output.Append("<ul>");

        //System.Web.Mvc.UrlHelper o = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);

        _object.Childs.OrderBy(x => x.BirthDate);
        foreach (CPerson subItem in _object.Childs)
        {
          output.Append("<li>");
          BuildPerson(baseUrl, subItem, output);
          IterationSubChilds(baseUrl, subItem, output);
          output.Append("</li>");
        }
        output.Append("</ul>");
      }
    }

    /// <summary>
    /// Partner data
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    public static void BuildParent(string baseUrl, CPerson person, StringBuilder output)
    {
      output.Append("<div>");
      PersonLinkWithName(person, output);
      output.Append("<br>");
      AddPersonImageLink(baseUrl, person, output);
      output.Append("<br>");
      OutputBirthText(person, output);
      //output.Append("</br>");
      OutputDeathText(person, output);
      output.Append("<br>");
      AddPartnerImageLink(baseUrl, person, output);
      AddParentImageLinks(person, output);
      AddRemarkLink(person, output);
      output.Append("</div>");
    }

    /// <summary>
    /// Person Data
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    public static void BuildPerson(string baseUrl, CPerson person, StringBuilder output)
    {
      output.Append("<div>");
      PersonLinkWithName(person, output);
      output.Append("<br>");
      AddPersonImageLink(baseUrl, person, output);
      output.Append("<br>");
      OutputBirthText(person, output);
      //output.Append("</br>");
      OutputDeathText(person, output);
      output.Append("<br>");
      AddPartnerImageLink(baseUrl, person, output);
      AddRemarkLink(person, output);
      output.Append("</div>");
    }

    /// <summary>
    /// Text der angezeigt werden soll
    /// </summary>
    /// <param name="subItem"></param>
    /// <returns></returns>
    public static string OutputPersonName(CPerson subItem)
    {
      return subItem.Fullname;
    }

    /// <summary>
    /// Geburtsdatum
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    public static void OutputBirthText(CPerson person, StringBuilder output)
    {
      output.Append("<em><small>* ");
      output.Append(person.BirthDisplay);
      output.Append("</small></em>");
    }

    /// <summary>
    /// Sterbedatum
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    public static void OutputDeathText(CPerson person, StringBuilder output)
    {
      if (person.IsDeath == true)
      {
        output.Append("<br>");
        output.Append("<em><small>† ");
        output.Append(person.DeathDisplay);
        output.Append("</small></em>");
      }
    }

    /// <summary>
    /// Create Person Image link ohne Name und Vorname
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    private static void AddPersonImageLink(string baseUrl, CPerson person, StringBuilder output)
    {
      string url = string.Empty;

      output.Append("<a");
      output.Append(" href=");
      output.Append('"');
      output.Append("FamilyTree?");
      output.Append("id=");
      output.Append(person.PersonID);
      output.Append('"');
      output.Append(">");

      output.Append("<img");
      output.Append(" class=");
      output.Append('"');
      output.Append("PersonImageSmall");
      output.Append('"');
      output.Append(" src=");

      url = "https://www.ahnenforschung.li/app_ahnenforschung/" + person.ImagePath.Replace("~/", "");
      output.Append('"' + url + '"');
      output.Append(">");
      output.Append("</a>");
    }

    /// <summary>
    /// Personen Link mit Name und Vorname
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    private static void PersonLinkWithName(CPerson person, StringBuilder output)
    {
      output.Append("<a");
      output.Append(" href=");
      output.Append('"');
      output.Append("FamilyTree?");
      output.Append("id=");
      output.Append(person.PersonID);
      output.Append('"');
      output.Append(">");
      output.Append(OutputPersonName(person));
      output.Append("</a>");
    }

    /// <summary>
    /// Link für eine Generation zurück
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    private static void AddParentImageLinks(CPerson person, StringBuilder output)
    {
      output.Append("<a");
      output.Append(" href=");
      output.Append('"');
      output.Append("FamilyTree?");
      output.Append("id=");
      if ((string)person.FatherID != "-1")
      {
        output.Append(person.FatherID);
      }
      else if ((string)person.MotherID != "-1")
      {
        output.Append(person.MotherID);
      }
      else
      {
        output.Append(person.PersonID);
      }
      output.Append('"');
      output.Append(">");
      ImageNavigationToParent(output);
      output.Append("</a>");
      output.Append("&nbsp;");
    }

    /// <summary>
    /// Link für die Partner
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    private static void AddPartnerImageLink(string baseUrl, CPerson person, StringBuilder output)
    {
      person.Partners.OrderByDescending(x => x.MarriageDateTime);
      foreach (appAhnenforschungData.Models.App.CPartner partner in person.Partners)
      {
        output.Append("<a");
        output.Append(" href=");
        output.Append('"');
        output.Append("FamilyTree?");
        output.Append("id=");
        output.Append(partner.Person.PersonID);
        output.Append('"');
        output.Append(">");
        ImageNavigationToPartner(baseUrl, partner.Person, output);
        output.Append("</a>");
        //output.Append("&nbsp;");
      }
    }

    /// <summary>
    /// Link zu weiteren Informationen zur Person
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    private static void AddRemarkLink(CPerson person, StringBuilder output)
    {
      output.Append("<a");
      //output.Append(" data-toggle=");
      //output.Append('"');
      //output.Append("modal");
      //output.Append('"');
      //output.Append(" data-target=");
      //output.Append('"');
      //output.Append("#modalRemarks");
      //output.Append('"');

      //output.Append(" appRemark (click)=");
      //output.Append('"');
      //output.Append("showRemarks(");
      //output.Append("'");
      //output.Append(person.PersonID);
      //output.Append("'");
      //output.Append(")");
      //output.Append('"');

      output.Append(" href=");
      output.Append('"');
      output.Append("PersonRemark?");
      output.Append("id=");
      output.Append(person.PersonID);
      output.Append('"');

      output.Append(">");
      ImageNavigationToRemark(person, output);
      output.Append("</a>");
      output.Append("&nbsp;");
    }

    /// <summary>
    /// Navigation zum Partner 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    private static void ImageNavigationToPartner(string baseUrl, CPerson person, StringBuilder output)
    {
      string url = string.Empty;


      //System.Web.Mvc.UrlHelper o = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
      output.Append("<img");
      output.Append(" style=");
      output.Append('"');
      output.Append("height:32px");
      output.Append('"');

      // https://stackoverflow.com/questions/40230473/how-to-serve-up-images-in-angular2
      output.Append(" src=");
      // url = o.Content("~/Content/Images/Design/AhnentafelNavigationZumPartner.png");
      //url = baseUrl + "/family-data/" + person.ImagePath.Replace("~/", "");
      url = "https://www.ahnenforschung.li/app_ahnenforschung/Content/Images/Design/AhnentafelNavigationZumPartner.png"; // + "src/app/family-data/AhnentafelNavigationZumPartner.png";
      output.Append('"' + url + '"');

      output.Append(" title=");
      output.Append('"');
      output.Append("Zum Partner ");
      output.Append(person.Fullname);
      output.Append(" wechseln");
      output.Append('"');
      output.Append(">"); // Close img
    }

    /// <summary>
    /// Navigation um eine Generation nach oben zu gelangen
    /// </summary>
    /// <param name="output"></param>
    private static void ImageNavigationToParent(StringBuilder output)
    {
      string url = string.Empty;

      output.Append("<img");
      output.Append(" style=");
      output.Append('"');
      output.Append("height:32px");
      output.Append('"');

      output.Append(" title=");
      output.Append('"');
      output.Append("Eine Generation nach oben navigieren");
      output.Append('"');

      output.Append(" src=");
      url = "https://www.ahnenforschung.li/app_ahnenforschung/Content/Images/Design/AhnentafelNavigationNachOben.png"; // + "src/app/family-data/AhnentafelNavigationZumPartner.png";
      output.Append('"' + url + '"');
      output.Append(">"); // Close img
    }

    /// <summary>
    /// Navigation zu weiteren Inforamtionen
    /// </summary>
    /// <param name="person"></param>
    /// <param name="output"></param>
    private static void ImageNavigationToRemark(CPerson person, StringBuilder output)
    {
      string url = string.Empty;

      output.Append("<img");
      output.Append(" style=");
      output.Append('"');
      output.Append("height:32px");
      output.Append('"');
      //output.Append(" ");

      output.Append(" title=");
      output.Append('"');
      output.Append("Weitere Informationen");
      output.Append('"');

      //output.Append(" ");
      output.Append(" alt=");
      output.Append('"');
      output.Append(person.PersonID);
      output.Append('"');
      //output.Append(" ");
      output.Append(" src=");
      url = "https://www.ahnenforschung.li/app_ahnenforschung/Content/Images/Design/Detail_32x32.png";
      output.Append('"' + url + '"');
      //output.Append(" ");
      output.Append(" class=");
      output.Append('"');
      output.Append("PersonDetail");
      output.Append('"');
      output.Append(">"); // Close img
    }

  }
}
