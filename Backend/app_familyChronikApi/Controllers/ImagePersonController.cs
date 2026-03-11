using appAhnenforschungBackEnd.Configutation;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace app_familyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagePersonController : Controller
    {

        private readonly ILogger<ImagePersonController> _logger;
        private readonly CSettings _oSettings;
        public ImagePersonController(ILogger<ImagePersonController> logger)
        {
            _logger = logger;
            _oSettings = new CSettings();
            _oSettings.UrlImagePath = ReadSettings.UrlRessources();
        }

        [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
        [HttpGet("GetAllImagePersons")]
        public IEnumerable<CImagePerson> GetAllImagePersons()
        {
            CReadWriteData oReadWriteData = new CReadWriteData();

            return oReadWriteData.GetAllImagePersons();
        }

        [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
        [HttpGet("GetActiveImagePersons")]
        public IEnumerable<CImagePerson> GetActiveImagePersons()
        {
            CReadWriteData oReadWriteData = new CReadWriteData();

            return oReadWriteData.GetActiveImagePersons();
        }

        [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
        [HttpGet("GetImagePersonWithPositionsById/{id}")]
        public CImagePerson GetImagePersonWihtPositionsById([FromRoute] int id)
        {
            CReadWriteData oReadWriteData = new CReadWriteData();

            return oReadWriteData.GetImagePersonWithPositionsById(id);
        }


        [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
        [HttpGet("GetImagePositionWithPositionsByPerson/{id}")]
        public IEnumerable<CImagePerson> GetImagePositionWithPositionsByPerson(string id)
        {
            CReadWriteData oReadWriteData = new CReadWriteData();

            return oReadWriteData.GetImagePositionWithPositionsByPerson(id);
        }



        [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
        [HttpPost("EditImagePerson/{id}")]
        public IActionResult EditImagePerson(int id, [FromBody] CImagePerson imagePerson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CReadWriteData oReadWriteData = new CReadWriteData();
            CImagePerson addedImagePerson = oReadWriteData.CreateOrUpdateImagePersons(imagePerson);

            if (addedImagePerson == null)
            {
                return NotFound();
            }
            return Ok(addedImagePerson);
        }

        [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
        [HttpPost("EditImagePersonPosition/{id}")]
        public IActionResult EditImagePersonPosition(int id, [FromBody] CImagePersonPosition imagePersonPostition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                CReadWriteData oReadWriteData = new CReadWriteData();
                List<CImagePersonPosition> imagePersonPostitions = new List<CImagePersonPosition>();
                imagePersonPostitions.Add(imagePersonPostition);
                oReadWriteData.CreateOrUpdateImagePersonPositions(imagePersonPostitions);

                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
        [HttpPost("AddImagePerson")]
        public IActionResult AddImagePerson([FromBody] CImagePerson imagePerson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            imagePerson.Add_Date = System.DateTime.Now;
            imagePerson.ImagePath = Path.Combine(_oSettings.UrlImagePath, "images//imagePerson");

            CReadWriteData oReadWriteData = new CReadWriteData();
            CImagePerson addedtImagePerson = oReadWriteData.CreateOrUpdateImagePersons(imagePerson);

            if (addedtImagePerson == null)
            {
                return NotFound();
            }
            return Ok(addedtImagePerson);
        }

        [Authorize(Roles = "Mitglied, EditAdress, EditMainPage, Admin")]
        [HttpPost("EditImagePersonPosition")]
        public IActionResult EditImagePersonPosition([FromBody] CImagePersonPosition imagePersonPosition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CReadWriteData oReadWriteData = new CReadWriteData();
            CImagePersonPosition addedtImagePersonPosition = oReadWriteData.UpdateImagePersonPosition(imagePersonPosition);

            if (addedtImagePersonPosition == null)
            {
                return NotFound();
            }
            return Ok(addedtImagePersonPosition);
        }
    }
}
