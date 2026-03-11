using Microsoft.AspNetCore.Mvc;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;


namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AddressesController : ControllerBase
  {
    CReadWriteData _context = new CReadWriteData();
    private readonly ILogger<AddressesController> _logger;

    public AddressesController(ILogger<AddressesController> logger)
    {
      _logger = logger;
    }

    // GET: api/Adresses/5
    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpGet("getAddressById/{id}")]
    public async Task<IActionResult> GetAddressById([FromRoute] int id)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var address = _context.GetAddressByID(id);

      if (address == null)
      {
        return NotFound();
      }

      return Ok(address);
    }

    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpGet("getAddressesByPersonId/{personId}")]
    public IEnumerable<CAddress> GetAddressesByPersonId([FromRoute] string personId)
    {
      var Address = _context.GetAddressesByPersonID(personId);

      return Address.ToList();
    }

    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("UpdateAddress/{id}")]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] CAddress address)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (id != address.AddressId)
      {
        return BadRequest();
      }

      _context.UpdateAddress(address);
      CAddress updateAddress = _context.GetAddressByID(id);


      if (id != updateAddress.AddressId)
      {
        return BadRequest();
      }
      return Ok(updateAddress);
    }

    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("UpdateAddresses/{rowsCount}")]
    public async Task<IActionResult> UpdateAddresses(int rowsCount, [FromBody] CAddress[] address)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //if (id != address.AddressId)
      //{
      //  return BadRequest();
      //}

      //try
      //{
      //  _context.UpdateAddress(address);
      //}
      //catch (DbUpdateConcurrencyException)
      //{
      //  return NotFound();
      //}

      return NoContent();
    }

    // POST: api/Adresses
    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("AddAddress")]
    public async Task<IActionResult> AddAddress([FromBody] CAddress address)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      _context.AddAddress(address);

      var Address = _context.GetAddressLastByPersonId(address.PersonId);
      if (Address == null)
      {
        return NotFound();
      }

      return Ok(Address);
    }

    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("deactivateAdress")]
    public async Task<IActionResult> DeactivateAdress([FromBody] CAddress address)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      _context.AddAddress(address);

      var Address = _context.GetAddressByID(address.AddressId);

      if (Address == null)
      {
        return NotFound();
      }
      Address.Active = false;
      _context.UpdateAddress(Address);

      return Ok(Address);
    }

    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("DeleteAdresses")]
    public async Task<IActionResult> DeleteAdresses([FromBody] CAddress address)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var Address = _context.GetAddressByID(address.AddressId);

      if (Address == null)
      {
        return NotFound();
      }
      _context.DeleteAddress(Address);

      return Ok(Address);
    }

  }
}