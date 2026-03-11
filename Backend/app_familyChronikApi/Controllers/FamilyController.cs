using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using appAhnenforschungBackEnd.Configutation;
using appAhnenforschungBackEnd.DataManager;
using appAhnenforschungBackEnd.Models;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class FamilyController : ControllerBase
  {

    private readonly ILogger<FamilyController> _logger;

    public FamilyController(ILogger<FamilyController> logger)
    {
      _logger = logger;
    }

    // GET: api/Family
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new string[] { "value1", "value2" };
    }


    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("{id}")]
    public IActionResult FamilysTreeString([FromRoute] string id)
    {
      string baseUrl = ReadSettings.UrlPersonImageSmall(); // "https://www.ahnenforschung.li/app_ahnenforschung/";

      CReadCacheData oReadCache = new CReadCacheData();
      appAhnenforschungData.DataManager.CReadWriteData oReaWrite = new appAhnenforschungData.DataManager.CReadWriteData();
      appAhnenforschungData.DataManager.CSettings oSettings = new appAhnenforschungData.DataManager.CSettings();
      CFam fam = new CFam();

      List<CPerson> arlChildren = new List<CPerson>();
      CPerson oParent = new CPerson();
      CPerson oParentFamily = new CPerson();

      oParent = oReadCache.GetCachedPersonByID(id, oSettings);

      oParentFamily = oReaWrite.GetFamilyPersonFromXml(oParent);

      if (oParentFamily == null)
      {

        oParent.Childs = new List<CPerson>();
        oParent.Partners = new List<CPartner>();

        arlChildren = oReadCache.GetCachedChildrenByPersonID(id, oSettings);

        // Die Partner zur Ersten Person
        foreach (CPartner partner in oReaWrite.GetPartnersByPersonID(id, oSettings))
        {
          System.Diagnostics.Debug.WriteLine(partner.PartnerID + " " + partner.Person.Fullname);
          oParent.Partners.Add(partner);
        }

        // Die Kinder zur Person
        foreach (CPerson person in arlChildren.OrderBy(x => x.tikBirth))
        {
          oParent.Childs.Add(person);
          person.Childs = new List<CPerson>();
          person.Partners = new List<CPartner>();

          // Die Partner zur Person
          foreach (CPartner partner in oReaWrite.GetPartnersByPersonID(person.PersonID, oSettings))
          {
            System.Diagnostics.Debug.WriteLine(partner.PartnerID + " " + partner.Person.Fullname);
            person.Partners.Add(partner);
          }

          IterationChildren(person, oReaWrite, oSettings);
        }

        oReaWrite.ModifiedFamilyPerson(oParent);
        fam.Parent = oParent;
        fam.InnerHtml = IterationHelper.ShowFamilyTree(baseUrl, fam.Parent).ToString();
      }
      else
      {
        fam.Parent = oParentFamily;
        fam.InnerHtml = IterationHelper.ShowFamilyTree(baseUrl, fam.Parent).ToString();
      }

      var family = fam;
      return Ok(family);
    }


    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("family/{id}")]
    public IActionResult FamilyTree([FromRoute] string id)
    {
      try
      {
        CReadCacheData oReadCache = new CReadCacheData();
        CReadWriteData oReaWrite = new CReadWriteData();
        CSettings oSettings = new CSettings();
        CFam fam = new CFam();

        List<CPerson> arlChildren = new List<CPerson>();
        CPerson oParent = new CPerson();
        CPerson oParentFamily = new CPerson();

        oSettings.UrlImagePath = ReadSettings.UrlPersonImageSmall();
        oParent = oReadCache.GetCachedPersonByID(id, oSettings);
        if (oParent.PersonID != null)
        {

          oParentFamily = oReaWrite.GetFamilyPersonFromXml(oParent);

          if (oParentFamily == null)
          {

            oParent.Childs = new List<CPerson>();
            oParent.Partners = new List<CPartner>();

            arlChildren = oReaWrite.GetChildrenByPersonID(id, oSettings);

            // Die Partner zur Ersten Person
            foreach (CPartner partner in oReaWrite.GetPartnersByPersonID(id, oSettings))
            {
              System.Diagnostics.Debug.WriteLine(partner.PartnerID + " " + partner.Person.Fullname);
              oParent.Partners.Add(partner);
            }

            // Die Kinder zur Person
            foreach (CPerson person in arlChildren.OrderBy(x => x.tikBirth))
            {
              oParent.Childs.Add(person);
              person.Childs = new List<CPerson>();
              person.Partners = new List<CPartner>();

              // Die Partner zur Person
              foreach (CPartner partner in oReaWrite.GetPartnersByPersonID(person.PersonID, oSettings))
              {
                System.Diagnostics.Debug.WriteLine(partner.PartnerID + " " + partner.Person.Fullname);
                person.Partners.Add(partner);
              }

              IterationChildren(person, oReaWrite, oSettings);
            }

            if (oParent.PersonID != null)
            {
              oReaWrite.ModifiedFamilyPerson(oParent);
            }
            fam.Parent = oParent;
          }
          else
          {
            fam.Parent = oParentFamily;
          }
        }
        var family = fam;
        return Ok(family);
      }
      catch(Exception ex)
      {
        return BadRequest(ex);
      }
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("deleteCache")]
    public IActionResult DeleteCache()
    {
      try
      {
        CReadWriteData oReaWrite = new CReadWriteData();
        oReaWrite.DeleteFamilyList();
        CApplicationSession.RemoveInstance();
        //return BadRequest(ModelState);
        //ReloadCache();
        var family = new CFam();
        return Ok(family);
      }
      catch (Exception ex)
      {
        return BadRequest(ex);
      }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("refreshKonshipConnection")]
    public IActionResult RefreshKonshipConnection()
    {
      try
      {
        CReadWriteData oReaWrite = new CReadWriteData();
        oReaWrite.DeleteKinshipConnection();
        oReaWrite.RefreshKinshipConnection();

        return Ok(true);
      }
      catch (Exception ex)
      {
        return BadRequest(ex);
      }
    }

    //public IActionResult FamilysTreeString([FromRoute] string id)
    //{
    //  string baseUrl = ReadSettings.UrlPersonImageSmall(); // "https://www.ahnenforschung.li/app_ahnenforschung/";
    //  // Microsoft.AspNetCore.Html.IHtmlContent
    //  CReadCacheData oReadCache = new CReadCacheData();
    //  CReadWriteData oReaWrite = new CReadWriteData();
    //  appAhnenforschungData.DataManager.CSettings oSettings = new appAhnenforschungData.DataManager.CSettings();
    //  CFam fam = new CFam();

    //  //oRead.DeleteFamilyList();
    //  List<CPerson> arlChildren = new List<CPerson>();
    //  CPerson oParent = new CPerson();
    //  CPerson oParentFamily = new CPerson();

    //  // oSettings = GetSettings();

    //  oParent = oReadCache.GetCachedPersonByID(id, oSettings);

    //  oParentFamily = oReaWrite.GetFamilyPersonFromXml(oParent);

    //  if (oParentFamily == null)
    //  {

    //    oParent.Childs = new List<CPerson>();
    //    oParent.Partners = new List<CPartner>();

    //    arlChildren = oReadCache.GetCachedChildrenByPersonID(id, oSettings);

    //    // Die Partner zur Ersten Person
    //    foreach (CPartner partner in oReaWrite.GetPartnersByPersonID(id, oSettings))
    //    {
    //      System.Diagnostics.Debug.WriteLine(partner.PartnerID + " " + partner.Person.Fullname);
    //      oParent.Partners.Add(partner);
    //    }

    //    // Die Kinder zur Person
    //    foreach (CPerson person in arlChildren.OrderBy(x => x.tikBirth))
    //    {
    //      oParent.Childs.Add(person);
    //      person.Childs = new List<CPerson>();
    //      person.Partners = new List<CPartner>();

    //      // Die Partner zur Person
    //      foreach (CPartner partner in oReaWrite.GetPartnersByPersonID(person.PersonID, oSettings))
    //      {
    //        System.Diagnostics.Debug.WriteLine(partner.PartnerID + " " + partner.Person.Fullname);
    //        person.Partners.Add(partner);
    //      }

    //      IterationChildren(person, oReaWrite, oSettings);
    //    }

    //    oReaWrite.ModifiedFamilyPerson(oParent);
    //    fam.Parent = oParent;
    //    fam.InnerHtml = IterationHelper.ShowFamilyTree(baseUrl, fam.Parent).ToString();
    //  }
    //  else
    //  {
    //    fam.Parent = oParentFamily;
    //    fam.InnerHtml = IterationHelper.ShowFamilyTree(baseUrl, fam.Parent).ToString();
    //  }

    //  var family = fam;
    //  return Ok(family);
    //}


    private void ReloadCache()
    {
      try
      {
        CReadCacheData oReadCache = new CReadCacheData();
        CReadWriteData oReadWrite = new CReadWriteData();
        CSettings oSettings = new CSettings();
        CFam fam = new CFam();

        foreach (CPerson pers in oReadWrite.GetPersons(oSettings))
        {
          //oRead.DeleteFamilyList();
          List<CPerson> arlChildren = new List<CPerson>();
          CPerson oParent = new CPerson();
          CPerson oParentFamily = new CPerson();

          oSettings.UrlImagePath = ReadSettings.UrlPersonImageSmall();
          oParent = oReadCache.GetCachedPersonByID(pers.PersonID, oSettings);
          if (oParent.PersonID != null)
          {

            oParentFamily = oReadWrite.GetFamilyPersonFromXml(oParent);

            if (oParentFamily == null)
            {

              oParent.Childs = new List<CPerson>();
              oParent.Partners = new List<CPartner>();

              arlChildren = oReadWrite.GetChildrenByPersonID(pers.PersonID, oSettings);

              // Die Partner zur Ersten Person
              foreach (CPartner partner in oReadWrite.GetPartnersByPersonID(pers.PersonID, oSettings))
              {
                System.Diagnostics.Debug.WriteLine(partner.PartnerID + " " + partner.Person.Fullname);
                oParent.Partners.Add(partner);
              }

              // Die Kinder zur Person
              foreach (CPerson person in arlChildren.OrderBy(x => x.tikBirth))
              {
                oParent.Childs.Add(person);
                person.Childs = new List<CPerson>();
                person.Partners = new List<CPartner>();

                // Die Partner zur Person
                foreach (CPartner partner in oReadWrite.GetPartnersByPersonID(person.PersonID, oSettings))
                {
                  System.Diagnostics.Debug.WriteLine(partner.PartnerID + " " + partner.Person.Fullname);
                  person.Partners.Add(partner);
                }

                IterationChildren(person, oReadWrite, oSettings);
              }

              if (oParent.PersonID != null)
              {
                oReadWrite.ModifiedFamilyPerson(oParent);
              }
              fam.Parent = oParent;
            }
            else
            {
              fam.Parent = oParentFamily;

            }
          }
        }
        //var family = fam;
        //return Ok(family);
      }
      catch (Exception)
      {
        throw;
      }
    }
 
   
 
    /// <summary>
    /// Iteration durch alle Objects
    /// </summary>
    /// <param name="oParent"></param>
    /// <param name="oRead"></param>
    /// <param name="oSettings"></param>
    private void IterationChildren(CPerson oParent, CReadWriteData oRead, CSettings oSettings)
    {

      foreach (CPerson child in oRead.GetChildrenByPersonID(oParent.PersonID, oSettings).OrderBy(x => x.tikBirth))
      {
        System.Diagnostics.Debug.WriteLine(child.PersonID + " " + child.Fullname);
        oParent.Childs.Add(child);
        child.Childs = new List<CPerson>();

        child.Partners = new List<CPartner>();
        foreach (CPartner partner in oRead.GetPartnersByPersonID(child.PersonID, oSettings))
        {
          System.Diagnostics.Debug.WriteLine(partner.PartnerID + " " + partner.Person.Fullname);
          child.Partners.Add(partner);

        }

        //child.Childs.Sort((x, y) => DateTime.Compare(x.BirthDate, y.BirthDate));
        //child.Partners.Sort((x, y) => DateTime.Compare(x.MarriageDateTime, y.MarriageDateTime));
        IterationChildren(child, oRead, oSettings);
      }
    }
  }
}
