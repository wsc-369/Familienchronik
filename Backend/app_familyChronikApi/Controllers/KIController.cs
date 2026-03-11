using app_familyBackend.KI;
using appAhnenforschungBackEnd.DataManager;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace app_familyBackend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class KIController : ControllerBase
  {
    private readonly ILogger<KIController> _logger;

    public KIController(ILogger<KIController> logger)
    {
      _logger = logger;
    }

    [HttpGet("GenerateTrainingDataFromDatabase")]
    public IActionResult GenerateTrainingDataFromDatabase([FromRoute] string id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      KIManager oManager = new KIManager();

      var person = oReadWriteData.GetPersonByID(int.Parse(id), appAhnenforschungBackEnd.DataManager.CGlobal.Settings());
      oManager.Init(appAhnenforschungBackEnd.DataManager.CGlobal.Settings());
      oManager.GenerateTrainingDataFromDatabase(person);

      return Ok();
    }

    [HttpGet("TrainModel")]
    public IActionResult TrainModel([FromRoute] string id)
    {
      KIManager oManager = new KIManager();
      oManager.Init(appAhnenforschungBackEnd.DataManager.CGlobal.Settings());
      oManager.TrainModel();

      return Ok();
    }

    [HttpGet("TestModel/{id}")]
    public IActionResult TestModel([FromRoute] string id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      KIManager oManager = new KIManager();
      oManager.Init(appAhnenforschungBackEnd.DataManager.CGlobal.Settings());
      var anver = oManager.QueryModel(oReadWriteData.GetPersonByID(id, appAhnenforschungBackEnd.DataManager.CGlobal.Settings()));

      return Ok(anver);
    }

    [HttpGet("Ask/{id}")]
    public IActionResult Ask([FromRoute] string id)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      KIManager oManager = new KIManager();
      oManager.Init(appAhnenforschungBackEnd.DataManager.CGlobal.Settings());
      var anver = oManager.QueryModel(oReadWriteData.GetPersonByID(id, appAhnenforschungBackEnd.DataManager.CGlobal.Settings()));

      return Ok(anver);
    }
  }
}
