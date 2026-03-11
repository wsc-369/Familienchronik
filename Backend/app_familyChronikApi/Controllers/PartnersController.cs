using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace appAhnenforschungBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {

    private readonly ILogger<PartnersController> _logger;

    public PartnersController(ILogger<PartnersController> logger)
    {
      _logger = logger;
    }

    [HttpGet("getPartners/{id}")]
    public IEnumerable<CPartner> getPartners([FromRoute] string id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      List<CPartner> arlPartners = new List<CPartner>();
      foreach (CPartner partner in oReadWriteData.GetPartnersByPersonID(id, DataManager.CGlobal.Settings()))
      {
        arlPartners.Add(partner);
      };

      return arlPartners.ToList();
    }

    [HttpGet("getPartner/{id}")]
    public IActionResult getPartner([FromRoute] string id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();

      var partner = oReadWriteData.GetPartnerByID(id, DataManager.CGlobal.Settings());
      if (partner == null)
      {
        return NotFound();
      }

      return Ok(partner);
    }

    // POST: api/Person
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddPartner([FromBody] CPartner partner)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      CPartner addedPartner = oReadWriteData.AddPartner(partner, DataManager.CGlobal.Settings());

      if (addedPartner == null)
      {
        return NotFound();
      }
      return Ok(addedPartner);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}")]
    public async Task<IActionResult> UpdatePartner(string id, [FromBody] CPartner partner)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      CPartner updatePartner = oReadWriteData.UpdatePartner(partner, DataManager.CGlobal.Settings());

      if (id != updatePartner.PartnerID)
      {
        return BadRequest();
      }
      return Ok(updatePartner);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("RemoveLinkPartnerFromPerson/{id}")]
    public async Task<IActionResult> RemoveLinkPartnerFromPerson(string id, [FromBody] CPartner partner)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      CReadWriteData oReadWriteData = new CReadWriteData();
      bool deleted = oReadWriteData.RemoveLinkPartnerFromPerson(partner);

      if (!deleted)
      {
        return BadRequest();
      }
      return Ok(deleted);
      }
    }
}
