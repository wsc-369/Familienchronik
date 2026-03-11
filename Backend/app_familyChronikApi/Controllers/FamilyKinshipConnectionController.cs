using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class FamilyKinshipConnectionController : Controller
  {

    private readonly ILogger<FamilyKinshipConnectionController> _logger;

    public FamilyKinshipConnectionController(ILogger<FamilyKinshipConnectionController> logger)
    {
      _logger = logger;
    }

    //[Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    //[HttpGet("getKinshipConnections")]
    //public IActionResult GetKinshipConnections([FromRoute] List<CPerson> persons)
    //{
    //  CReadWriteData oReadWriteData = new CReadWriteData();
    //  List<CPerson> oKinshipConnections = oReadWriteData.GetKinshipConnections(persons, DataManager.CGlobal.Settings());

    //  if (oKinshipConnections == null)
    //  {
    //    return NotFound();
    //  }

    //  return Ok(oKinshipConnections);
    //}

    //[Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    //[HttpPost("searchKinshipConnections")]
    //public IActionResult PostKinshipConnections([FromRoute] CPerson persons)
    //{
    //  CReadWriteData oReadWriteData = new CReadWriteData();
    //  //List<CPerson> oKinshipConnections = oReadWriteData.GetKinshipConnections(persons, DataManager.CGlobal.Settings());

    //  //if (oKinshipConnections == null)
    //  //{
    //  //  return NotFound();
    //  //}

    //  //return Ok(oKinshipConnections);
    //  return Ok();
    //}

    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("filtered/{filterParam}")]
    public IEnumerable<CPerson> GetFiltered(string filterParam)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      List<CPerson> arlPersons = new List<CPerson>();
      //CPerson person;
      //bool hasfiltered = false;

      //string[] filters = filterParam.Split(";");
      //for (int i = 0; i < filters.Length -1; i++)
      //{
      //  person = oReadWriteData.GetPersonByID(filters[i], DataManager.CGlobal.Settings());
      //  if (person != null && person.PersonID !="")
      //  {
      //    arlPersons.Add(person);
      //  }
      //}
      //if (arlPersons.Count > 0)
      //{
        arlPersons = oReadWriteData.GetKinshipConnections(filterParam, DataManager.CGlobal.Settings());
      //}
      return arlPersons.ToList();
    }

  }
}
