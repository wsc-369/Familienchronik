using app_familyChronikApi.ReadWriteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValueObject;

namespace app_familyBackend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DialectController : ControllerBase
  {

    private readonly ReadWiteDialects _reader;
    private readonly ILogger<DialectController> _logger;

    public DialectController(ReadWiteDialects reader, ILogger<DialectController> logger)
    {
      _reader = reader;
      _logger = logger;
    }

    [HttpGet("getDialectWords")]
    public async Task<IEnumerable<DialectWord>> GetDialectWords(CancellationToken cancellationToken)
    {
      return await _reader.GetDialectWords(onlyActive: true, token: cancellationToken);
    }

    [HttpGet("getDialectWord/{id}")]
    public async Task<ValueObject.DialectWord> GetDialectWord([FromRoute] string id, CancellationToken cancellationToken)
    {
      if (!Guid.TryParse(id, out Guid guid)) return null;

      return await _reader.GetDialectWord(id: guid, token: cancellationToken);
    }


    [HttpGet("getEmptyDialectWord")]
    public async Task<ValueObject.DialectWord> GetEmptyDialectWord(CancellationToken cancellationToken)
    {
       return await _reader.GetEmptyDialectWord();
    }

    [HttpGet("getDialectFilteredWords/{word}")]
    public async Task<IEnumerable<DialectWord>> getDialectFilteredWords(string word, CancellationToken cancellationToken)
    {
      return await _reader.GetDialectWords(onlyActive: true, token: cancellationToken);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("updateDialectWord/{id}")]
    public async Task<IActionResult> UpdateDialectWord(string id, [FromBody] DialectWord dialect, CancellationToken cancellationToken)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return BadRequest(ModelState);
        }

        if (!Guid.TryParse(id, out Guid guid)) return BadRequest();
        if (guid != dialect.Id)
        {
          return BadRequest();
        }

        var update =  await _reader.UpdateDialectWord(dialect, token: cancellationToken);

        return Ok(update);
        
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in UpdateDialectWord");
        return BadRequest();
      }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("addDialectWord")]
    public async Task<IActionResult> AddDialectWord([FromBody] DialectWord dialect, CancellationToken cancellationToken)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return BadRequest(ModelState);
        }


        var update = await _reader.AddDialectWord(dialect, token: cancellationToken);

        return Ok(update);

      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in AddDialectWord");
        return BadRequest();
      }
    }
  }
}
