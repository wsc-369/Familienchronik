using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace appAhnenforschungBackEnd.Controllers
{
    [Route("api/[controller]")]
  [ApiController]
  public class EmployeesController : ControllerBase
  {

    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(ILogger<EmployeesController> logger)
    {
      _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IEnumerable<CUser> GetEmployee()
    {
      CReadWriteData oReadWriteData = new CReadWriteData();

      //var Message = $"About page visited at {DateTime.UtcNow.ToLongTimeString()}";
      //_logger.LogInformation(Message);
      return oReadWriteData.GetUsers().ToList(); //_context.tblemployee;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee([FromRoute] int id)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();

      var employee = oReadWriteData.GetUserByID(id);//.ToList(); //_context.tblemployee;

      //var employee = await _context.tblemployee.FindAsync(id);

      if (employee == null)
      {
        return NotFound();
      }

      return Ok(employee);
    }

    //[Authorize(Roles = "Admin")]
    //[HttpPut("{id}")]
    //public async Task<IActionResult> AddEmployee([FromBody] CUser employee)
    //{
    //  if (!ModelState.IsValid)
    //  {
    //    return BadRequest(ModelState);
    //  }

    //  CReadWriteData oReadWriteData = new CReadWriteData();
    //  CUser updateUser = oReadWriteData.AddUser(employee);

    //  if (id != updateUser.UserId)
    //  {
    //    return BadRequest();
    //  }
    //  return Ok(updateUser);
    //}

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] CUser employee)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      CUser updateUser = oReadWriteData.UpdateUser(employee);

      if (id != updateUser.UserId)
      {
        return BadRequest();
      }
      return Ok(updateUser);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddEmployee([FromBody] CUser employee)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      CUser addedUser = oReadWriteData.AddUser(employee);

      if (addedUser == null)
      {
        return NotFound();
      }
      return Ok(addedUser);
    }
  }
}