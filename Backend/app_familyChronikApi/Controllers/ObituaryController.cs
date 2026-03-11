using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ObituaryController : ControllerBase
    {

    private readonly ILogger<ObituaryController> _logger;

    public ObituaryController(ILogger<ObituaryController> logger)
    {
      _logger = logger;
    }

    CReadWriteData _context = new CReadWriteData();

    [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
    [HttpGet("getObituary/{id}")]
    public IActionResult getObituary([FromRoute] string id)
    {
      CObituary oObituary = new CObituary();
      oObituary = _context.GetObituaryByPersonID(id);

      if (oObituary == null)
      {
        return NotFound();
      }

      return Ok(oObituary);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("AddObituary")]
    public async Task<IActionResult> AddObituary([FromBody] CObituary obituary)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CObituary addedObituary = _context.AddObituary(obituary);

      if (addedObituary == null)
      {
        return NotFound();
      }
      return Ok(addedObituary);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("UpdateObituary/{id}")]
    public async Task<IActionResult> UpdateObituary(string id, [FromBody] CObituary obituary)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CObituary updateObituary = _context.UpdateObituary(obituary);

      if (id != updateObituary.PersonID)
      {
        return BadRequest();
      }
      return Ok(updateObituary);
    }



  }
}