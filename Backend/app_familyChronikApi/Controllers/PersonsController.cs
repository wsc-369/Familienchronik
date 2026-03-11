using app_familyBackend.MigrationOfData;
using app_familyChronikApi.ReadWriteDB;
using appAhnenforschungBackEnd.Filters;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValueObject;
using static appAhnenforschungBackEnd.DataManager.CGlobal;

namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PersonsController : ControllerBase
  {

    private readonly ReadPersonRelations _reader;
    private readonly DoMigratonData _dataMigration;
    private readonly ReadWirtePersons _readWritePersons;
    private readonly ILogger<PersonsController> _logger;

    public PersonsController(DoMigratonData dataMigration, ReadPersonRelations reader, ReadWirtePersons readWritePersons, ILogger<PersonsController> logger)
    {
      _dataMigration = dataMigration;
      _reader = reader;
      _readWritePersons = readWritePersons;
      _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("DoMigratDataPersons")]
    public async Task<ActionResult<bool>> DoMigratDataPersons(CancellationToken cancellationToken)
    {


      //var ok = await _reader.AddAllPersonRelations(cancellationToken);
      //if (ok)
      //{
      //  return Ok(ok);
      //}

      /// Vorgängige Migration rückgängig machen

      var ok = await _dataMigration.DoMigratUsers(cancellationToken);
      if (ok)
      {
        ok = await _dataMigration.UndoMigratDataPersons(cancellationToken);
        if (ok)
        {
          /// Dann neue Migration durchführen
          ok = await _dataMigration.DoMigratDataPersons(cancellationToken);
          if (ok)
          {
            ok = await _dataMigration.DoMigratDialectWordCollection(cancellationToken);
            if (ok)
            {
              ok = await _dataMigration.DoMigratWIFI(cancellationToken);
              if (ok)
              {
                ok = await _dataMigration.DoMigratTemplates(cancellationToken);
                if (ok)
                {
                  ok = await _dataMigration.DoMigratPersonAddresses(cancellationToken);
                  return Ok(ok);
                  
                  ok = await _reader.AddAllPersonRelations(cancellationToken);
                  if (ok)
                  {
                    return Ok(ok); /// Alles erfolgreich
                  }
                }
              }
            }
          }

          ///// Danach alle Personen Beziehungen hinzufügen
          //ok = await _reader.AddAllPersonRelations(cancellationToken);
          //if (ok)
          //{
          //  return Ok(ok); /// Alles erfolgreich
          //}
        }
      }
      // / Etwas ist schief gelaufen
      return BadRequest();
    }


    //[Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<CPerson>>> GetAllPersonsAsync(CancellationToken cancellationToken)
    //{
    //  var persons = await _reader.GetAllPersonsAsync(cancellationToken);
    //  return Ok(persons);
    //}

    //[Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    //[HttpGet]
    //public IEnumerable<CPerson> GetPersons()
    //{
    //  CReadWriteData oReadWriteData = new CReadWriteData();

    //  return oReadWriteData.GetPersons(new appAhnenforschungData.DataManager.CSettings()).ToList();
    //}


    [Authorize(Roles = AppUserRoleAdmin)]
    [HttpGet("getPersonByQuery")]
    public async Task<IEnumerable<TypeheadPerson>> GetPersonByQuery(string familyName, string firstName, CancellationToken cancellationToken)
    {
      return await _reader.GetPersonByQuery(familyName, firstName, cancellationToken);
    }

    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("GetPersonNamesGrouped")]
    public IEnumerable<String> GetPersonNamesGrouped()
    {
      return _readWritePersons.GroupedFamilyNames();
    }

    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("GetPersonPreNamesGrouped")]
    public IEnumerable<String> GetPersonPreNamesGrouped()
    {
      return _readWritePersons.GroupedFirstNames();
    }

    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPersons([FromRoute] string id, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var result = await _readWritePersons.GetPersonByRefId(id, cancellationToken);

      if (result == null)
      {
        return NotFound();
      }

      return Ok(result);

      CReadWriteData oReadWriteData = new CReadWriteData();

      var employee = oReadWriteData.GetPersonByID(id, new CSettings());

      //var employee = await _context.tblemployee.FindAsync(id);

      if (employee == null)
      {
        return NotFound();
      }

      return Ok(employee);
    }


    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("getPersonRefId/{id}")]
    public async Task<IActionResult> GetPersonRefId([FromRoute] string id, CancellationToken cancellationToken)
    {
      bool isValid = Guid.TryParse(id, out Guid result);
      if (!isValid)
      {
        return BadRequest("Invalid GUID format.");
      }
      var personRefId = await _readWritePersons.GetPersonRefId(result, cancellationToken);


      CReadWriteData oReadWriteData = new CReadWriteData();
      var person = oReadWriteData.GetPersonByID(personRefId, new CSettings());

      if (person == null)
      {
        return NotFound();
      }

      return Ok(person);
    }





    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("getChildren/{id}")]
    public IEnumerable<CPerson> getChildren([FromRoute] string id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      List<CPerson> arlChildrens = new List<CPerson>();
      arlChildrens = oReadWriteData.GetChildrenByPersonID(id, new CSettings());

      return arlChildrens.ToList();
    }


    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("getPartners/{id}")]
    public IEnumerable<CPartner> getPartners([FromRoute] string id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      List<CPartner> arlPartners = new List<CPartner>();
      foreach (CPartner partner in oReadWriteData.GetPartnersByPersonID(id, new CSettings()))
      {
        arlPartners.Add(partner);
      }
      ;

      return arlPartners.ToList();
    }


    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("getPartner/{id}")]
    public IActionResult getPartner([FromRoute] string id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();

      var partner = oReadWriteData.GetPartnerByID(id, new CSettings());
      if (partner == null)
      {
        return NotFound();
      }

      return Ok(partner);
    }


    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("getPartnerById/{id}")]
    public IActionResult getPartnerById([FromRoute] string id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();

      var partner = oReadWriteData.GetPartnerByID(id, new CSettings());
      if (partner == null)
      {
        return NotFound();
      }

      return Ok(partner);
    }



    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("getFiltered/{filterParam}")]
    public async Task<IEnumerable<Person>> GetFiltered(string filterParam)
    {

      IEnumerable<ValueObject.Person> persons = null;
      FilterPersons oFilterPersons = new FilterPersons();
      List<CPerson> arlPersons = new List<CPerson>();
      List<CPerson> arlPersonsPreName = new List<CPerson>();
      List<CPerson> arlPersonsFirstName = new List<CPerson>();
      bool hasfiltered = false;

      string[] filters = filterParam.Split(";");
      for (int i = 0; i < filters.Length; i++)
      {
        string[] filter = filters[i].Split("_");
        switch (i)
        {
          case 0:
            if (filter[0] == "personID" && filter[1] != "undefined")
            {
              oFilterPersons.personID = filter[1].ToString();
              hasfiltered = true;
            }
            break;
          case 1:
            if (filter[0] == "firstName" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.firstName  = filter[1].ToString();
              hasfiltered = true;
            }
            break;
          case 2:
            if (filter[0] == "familyName" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.familyName = filter[1].ToString();
              hasfiltered = true;
            }
            break;
          case 3:
            if (filter[0] == "birthDate" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.birthDate = filter[1].ToString();
              hasfiltered = true;
            }
            break;
          case 4:
            if (filter[0] == "deathDate" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.deathDate = filter[1].ToString();
              hasfiltered = true;
            }
            break;

          case 5:
            if (filter[0] == "birthYear" && filter[1].ToString() != "undefined" && filter[1].ToString() != "0")
            {
              oFilterPersons.birthYear = filter[1].ToString();
              hasfiltered = true;
            }
            break;

          case 6:
            if (filter[0] == "older" && filter[1].ToString() != "undefined" && filter[1].ToString() != "0")
            {
              oFilterPersons.older = filter[1].ToString();
              hasfiltered = true;
            }
            break;




          case 7:
            if (filter[0] == "dateFrom" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.dateFrom = filter[1].ToString();
              hasfiltered = true;
            }
            break;

          case 8:
            if (filter[0] == "dateUntil" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.dateUntil = filter[1].ToString();
            }
            break;

          case 9:
            if (filter[0] == "wildCardText" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.wildCardText = filter[1].ToString();
              hasfiltered = true;
            }
            break;

            //default:
            //    Console.WriteLine("Default case");
            //    arlPersons = oReadWriteData.GetPersonWildcardFilterByPersonId("I-100", new CSettings());
            //    i = filters.Length;
            //    break;
        }
      }
      if (hasfiltered == true)
      {
        persons = await _reader.GetPersonWildcardFilter(oFilterPersons, new CSettings());
      }
      else
      {
        oFilterPersons.personID = "I-100";
        persons = await _reader.GetPersonWildcardFilterByPersonId(oFilterPersons, new CSettings());
      }

      return persons;
    }





    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("filtered/{filterParam}")]
    public async Task<IEnumerable<CPerson>> GetFiltered1(string filterParam, CancellationToken cancellationToken)
    {

      CReadWriteData oReadWriteData = new CReadWriteData();

      FilterPersons oFilterPersons = new FilterPersons();
      List<CPerson> arlPersons = new List<CPerson>();
      List<CPerson> arlPersonsPreName = new List<CPerson>();
      List<CPerson> arlPersonsFirstName = new List<CPerson>();
      bool hasfiltered = false;

      string[] filters = filterParam.Split(";");
      for (int i = 0; i < filters.Length; i++)
      {
        string[] filter = filters[i].Split("_");
        switch (i)
        {
          case 0:
            if (filter[0] == "personID" && filter[1] != "undefined")
            {
              oFilterPersons.personID = filter[1].ToString();
              hasfiltered = true;
            }
            break;
          case 1:
            if (filter[0] == "firstName" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.firstName = filter[1].ToString();
              hasfiltered = true;
            }
            break;
          case 2:
            if (filter[0] == "familyName" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.familyName = filter[1].ToString();
              hasfiltered = true;
            }
            break;

          case 3:
            if (filter[0] == "birthDate" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.birthDate = filter[1].ToString();
              hasfiltered = true;
            }
            break;
          case 4:
            if (filter[0] == "deathDate" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.deathDate = filter[1].ToString();
              hasfiltered = true;
            }
            break;

          case 5:
            if (filter[0] == "birthYear" && filter[1].ToString() != "undefined" && filter[1].ToString() != "0")
            {
              oFilterPersons.birthYear = filter[1].ToString();
              hasfiltered = true;
            }
            break;

          case 6:
            if (filter[0] == "older" && filter[1].ToString() != "undefined" && filter[1].ToString() != "0")
            {
              oFilterPersons.older = filter[1].ToString();
              hasfiltered = true;
            }
            break;




          case 7:
            if (filter[0] == "dateFrom" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.dateFrom = filter[1].ToString();
              hasfiltered = true;
            }
            break;

          case 8:
            if (filter[0] == "dateUntil" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.dateUntil = filter[1].ToString();
            }
            break;

          case 9:
            if (filter[0] == "wildCardText" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.wildCardText = filter[1].ToString();
              hasfiltered = true;
            }
            break;

            //default:
            //    Console.WriteLine("Default case");
            //    arlPersons = oReadWriteData.GetPersonWildcardFilterByPersonId("I-100", new CSettings());
            //    i = filters.Length;
            //    break;
        }
      }
      if (hasfiltered == true)
      {
        arlPersons = oReadWriteData.GetPersonWildcardFilter(oFilterPersons, new CSettings());
      }
      else
      {
        arlPersons = oReadWriteData.GetPersonWildcardFilterByPersonId("I-100", new CSettings());
      }
      if (arlPersons.Count == 1)
      {
        await _reader.AddPersonRelationByPerson(arlPersons[0].PersonID, cancellationToken);
        //  KIManager oKIManager = new KIManager();
        //  oKIManager.Init(DataManager.CGlobal.Settings());
        //  //oKIManager.GenerateTrainingDataFromDatabase(arlPersons[0]);
        // // oKIManager.TrainModel();
        //  oKIManager.QueryModel(arlPersons[0]);
      }
      return arlPersons.ToList();
    }

    /// <summary>
    /// Filter by pre name with wildcard
    /// </summary>
    /// <param name="filterParam"></param>
    /// <returns></returns>
    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("filteredByPreName/{filterParam}")]
    public IEnumerable<CPerson> GetFilteredByName(string filterParam)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      FilterPersons oFilterPersons = new FilterPersons();
      List<CPerson> arlPersons = new List<CPerson>();

      oFilterPersons.familyName = filterParam.ToString();
      arlPersons = oReadWriteData.GetPersonWildcardFilter(oFilterPersons, new CSettings());

      return arlPersons.ToList();
    }

    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("get")]
    public IEnumerable<CPerson> Get([FromRoute] string id)
    {
      {

        CReadWriteData oReadWriteData = new CReadWriteData();

        return oReadWriteData.GetPersonWildcardFilterByPersonId(id, new CSettings()).ToList();
      }
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("addPerson")]
    public async Task<IActionResult> AddPerson([FromBody] CPerson person)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      CPerson addedPerson = oReadWriteData.AddPerson(person, new CSettings());

      if (addedPerson == null)
      {
        return NotFound();
      }
      return Ok(addedPerson);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("{id}")]
    public async Task<IActionResult> UpdatePerson(string id, [FromBody] Person person, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      bool isValid = Guid.TryParse(id, out Guid result) && person.Id != Guid.Empty;
      if (!isValid)
      {
        return BadRequest("Invalid GUID format.");
      }
      var updatePerson = await _readWritePersons.UpdatePersonByRefId(result, person, cancellationToken);

      return Ok(updatePerson);


     // CReadWriteData oReadWriteData = new CReadWriteData();
      //CPerson updatePerson = oReadWriteData.UpdatePerson(person, DataManager.CGlobal.Settings());

      //if (id != updatePerson.PersonID)
      //{
      //  return BadRequest();
      //}
      //return Ok(updatePerson);

    //  return BadRequest();
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("UpdatePartnerLink/{id}")]
    public async Task<IActionResult> UpdatePartnerLink(string id, [FromBody] CPartner partner)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      CPartner updatePartner = oReadWriteData.UpdatePartnerLink(partner, DataManager.CGlobal.Settings());

      if (id != updatePartner.PartnerID)
      {
        return BadRequest();
      }
      return Ok(updatePartner);
    }

    // DELETE: api/ApiWithActions/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
