using appAhnenforschungBackEnd.Models;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PersonPortraitController : ControllerBase
  {

    private readonly ILogger<PersonPortraitController> _logger;

    public PersonPortraitController(ILogger<PersonPortraitController> logger)
    {
      _logger = logger;
    }

    // GET: api/Remarks
    //[Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    //[HttpGet("getPersonPortrait/{id}")]
    //public IActionResult getPersonPortrait([FromRoute] string id)
    //{
    //  CReadWriteData oReadWriteData = new CReadWriteData();
    //  CPersonPortrait oPortrait = new CPersonPortrait();
    //  oPortrait = oReadWriteData.GetPersonPortraitByPersonID(id);

    //  if (oPortrait == null)
    //  {
    //    return NotFound();
    //  }

    //  return Ok(oPortrait);
    //}

    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("getPersonPortraits/{id}")]
    public IActionResult getPersonPortraits([FromRoute] string id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      List<CPersonPortrait>oPortraits = oReadWriteData.GetPersonPortraitsByPersonID(id);

      if (oPortraits == null)
      {
        return NotFound();
      }

      return Ok(oPortraits);
    }

    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("addPersonPortrait")]
    public async Task<IActionResult> AddPersonPortrait([FromBody] CPersonPortrait personPortrait)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      CPersonPortrait addPersonPortrait = oReadWriteData.AddPersonPortrait(personPortrait);

      if (addPersonPortrait == null)
      {
        return NotFound();
      }
      return Ok(addPersonPortrait);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("UpdatePersonPortrait/{id}")]
    public async Task<IActionResult> UpdatePersonPortrait(string id, [FromBody] CPersonPortrait personPortrait)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      CPersonPortrait updatePersonPortrait = oReadWriteData.UpdatePersonPortrait(personPortrait);

      if (id != updatePersonPortrait.PersonID)
      {
        return BadRequest();
      }
      return Ok(updatePersonPortrait);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("DeletePersonPortrait")]
    public async Task<IActionResult> DeletePersonPortrait([FromBody] CPersonPortrait personPortrait)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      bool isDeleted = oReadWriteData.DeletePersonPortrait(personPortrait);

      if (!isDeleted)
      {
        return BadRequest();
      }
      return Ok(personPortrait);
    }

    // DELETE: api/ApiWithActions/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      CPersonPortrait personPortrait = oReadWriteData.GetPersonPortraitByID(id);
      personPortrait.Active = false;
      oReadWriteData.UpdatePersonPortrait(personPortrait);
    }

    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("UploadPortrait/{id}"), DisableRequestSizeLimit]
    public IActionResult UploadPortrait(int id)
    {
      try
      {
        var file = Request.Form.Files[0];
        var folderName = Path.Combine("resources", "documents");
        folderName = Path.Combine(folderName, "portraits");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        if (!Directory.Exists(pathToSave))
        {
          Directory.CreateDirectory(pathToSave);
        }
        
        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var extention = Path.GetExtension(fileName);

          CReadWriteData oReadWriteData = new CReadWriteData();
          CPersonPortrait personPortrait = oReadWriteData.GetPersonPortraitByID(id);

          var fullPath = Path.Combine(pathToSave, fileName);
          var dbPath = Path.Combine(folderName, fileName);
          using (var stream = new FileStream(fullPath, FileMode.Create))
          {
            file.CopyTo(stream);
            personPortrait.PdfName = fileName;
            personPortrait.Update = DateTime.Now;
            oReadWriteData.UpdatePersonPortrait(personPortrait);
          }

          return Ok(new { dbPath });
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }
  }
}
