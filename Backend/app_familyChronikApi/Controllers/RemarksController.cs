using System.Collections.Generic;
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
    public class RemarksController : ControllerBase
    {

        private readonly ILogger<RemarksController> _logger;

        public RemarksController(ILogger<RemarksController> logger)
        {
            _logger = logger;
        }

        // GET: api/Remarks
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
        [HttpGet("getRemark/{id}")]
        public IActionResult getRemark([FromRoute] string id)
        {
            CReadWriteData oReadWriteData = new CReadWriteData();
            CRemark oRemark = new CRemark();
            oRemark = oReadWriteData.GetRemarkByPersonID(id);

            if (oRemark == null)
            {
                return NotFound();
            }

            return Ok(oRemark);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddRemark")]
        public async Task<IActionResult> AddRemark([FromBody] CRemark remark)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CReadWriteData oReadWriteData = new CReadWriteData();
            CRemark addedRemark = oReadWriteData.AddRemark(remark);

            if (addedRemark == null)
            {
                return NotFound();
            }
            return Ok(addedRemark);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("UpdateRemark/{id}")]
        public async Task<IActionResult> UpdateRemark(string id, [FromBody] CRemark remark)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CReadWriteData oReadWriteData = new CReadWriteData();
            CRemark updateRemark = oReadWriteData.UpdateRemark(remark);

            if (id != updateRemark.PersonID)
            {
                return BadRequest();
            }
            return Ok(updateRemark);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutRemark([FromRoute] int id, [FromBody] CRemark remark)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //CReadWriteData oReadWriteData = new CReadWriteData();
            //CRemark updatePerson = oReadWriteData.UpdateRem n(person, new CSettings());

            //if (id != updatePerson.ID)
            //{
            //  return BadRequest();
            //}
            //return Ok(updatePerson);

            return BadRequest();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
