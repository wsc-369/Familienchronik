using app_familyBackend.DataContext;
using app_familyBackend.DataManager;
using app_familyChronikApi.ReadWriteDB;
using appAhnenforschungBackEnd.Models;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
//using static System.Net.Mime.MediaTypeNames;

namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ContentTemplatesController : ControllerBase
  {
    private readonly ILogger<ContentTemplatesController> _logger;

    private readonly MyDatabaseContext _context;

    private readonly ReadWirteContents _readWirteContents;

    public ContentTemplatesController(MyDatabaseContext context, ReadWirteContents readWirteContents, ILogger<ContentTemplatesController> logger)
    {
      _logger = logger;
      _context = context;
      _readWirteContents = readWirteContents;
    }


    [HttpGet]
    public async Task<IEnumerable<ValueObject.ContentTemplate>> GetContendTemplates()
    {
      return await _readWirteContents.GetContentTemplates();
    }

    [HttpGet("getContentTemplatesByType/{id}")]
    public async Task<IEnumerable<ValueObject.ContentTemplate>> GetContendTemplatesByType([FromRoute] int id)
    {

      return await _readWirteContents.GetContendTemplatesByType(id);
    }

    

    [Authorize(Roles = "Admin")]
    [HttpGet("getContentTemplate/{id}")]
    public async Task<ValueObject.ContentTemplate> getContentTemplate([FromRoute] string id)
    {
      var template = await _readWirteContents.GetSingleContentTemplate(Guid.Parse(id));

     
      return template;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getEmptyContentTemplateLink")]
    public async Task<ValueObject.ContentTemplateLink> getEmptyContentTemplateLink([FromRoute] string id)
    {

      var template = _readWirteContents.EmptyContentTemplateLink();

      return template;

    }

    [Authorize(Roles = "Admin")]
    [HttpPost("addContentTemplate")]
    public async Task<IActionResult> AddContentTemplate([FromBody] ValueObject.ContentTemplate contentTemplate)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var template = await _readWirteContents.AddContentTemplate(contentTemplate);
     
      if (template == null)
      {
        return NotFound();
      }
      return Ok(template);
    }

   // [Authorize(Roles = "Admin")]
    [HttpPost("addContentTemplateLink")]
    public async Task<IActionResult> AddContentTemplateLink([FromBody] ValueObject.ContentTemplateLink contentTemplateLink)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var template = await _readWirteContents.AddContentTemplate(contentTemplateLink);

      if (template == null)
      {
        return NotFound();
      }
      return Ok(template);
      //if (!ModelState.IsValid)
      //{
      //  return BadRequest(ModelState);
      //}

      //CReadWriteData oReadWriteData = new CReadWriteData();
     // CContentTemplateLink addedContentTemplateLink = oReadWriteData.AddContentTemplateLink(contentTemplateLink);

      //if (addedContentTemplateLink == null)
      //{
      //  return NotFound();
      //}
      //return Ok(addedContentTemplateLink);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("addContentTemplateImage")]
    public async Task<IActionResult> AddContentTemplateImage([FromBody] ValueObject.ContentTemplateImage contentTemplateImage)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var template = await _readWirteContents.AddContentTemplateImage(contentTemplateImage);

      if (template == null)
      {
        return NotFound();
      }
      return Ok(template);

      //if (!ModelState.IsValid)
      //{
      //  return BadRequest(ModelState);
      //}

      //CReadWriteData oReadWriteData = new CReadWriteData();
      //CContentTemplateImage addedContentTemplateImage = oReadWriteData.AddContentTemplateImage(contentTemplateImage);

      //if (addedContentTemplateImage == null)
      //{
      //  return NotFound();
      //}
      //return Ok(addedContentTemplateImage);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("updateContentTemplate/{id}")]
    public async Task<IActionResult> UpdateContentTemplate(string id, [FromBody] ValueObject.ContentTemplate contentTemplate)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var template = await _readWirteContents.UpdateContentTemplate(contentTemplate);

      if (template == null)
      {
        return NotFound();
      }
      return Ok(template);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("updateContentTemplateLink/{id}")]
    public async Task<IActionResult> UpdateContentTemplateLink(string id, [FromBody] ValueObject.ContentTemplateLink contentTemplateLink)
    {

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var template = await _readWirteContents.UpdatetContentTemplateLink(contentTemplateLink);

      if (!template)
      {
        return NotFound();
      }
      
      return Ok(template);

      //if (!ModelState.IsValid)
      //{
      //  return BadRequest(ModelState);
      //}

      //CReadWriteData oReadWriteData = new CReadWriteData();
      //CContentTemplateLink update = oReadWriteData.UpdateContentTemplateLink(contentTemplateLink);

      //if (id != update.ContentTemplateLinkId.ToString())
      //{
      //  return BadRequest();
      //}
      //return Ok(update);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("updateContentTemplateImage/{id}")]
    public async Task<IActionResult> UpdateContentTemplateImage(string id, [FromBody] ValueObject.ContentTemplateImage contentTemplateImage)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var template = await _readWirteContents.UpdatetContentTemplateImage(contentTemplateImage);

      if (!template)
      {
        return NotFound();
      }

      return Ok(template);
      //if (!ModelState.IsValid)
      //{
      //  return BadRequest(ModelState);
      //}

      //CReadWriteData oReadWriteData = new CReadWriteData();
      //CContentTemplateImage update = oReadWriteData.UpdateContentTemplateImage(contentTemplateImage);

      //if (id != update.ContentTemplateImageId.ToString())
      //{
      //  return BadRequest();
      //}
      //return Ok(update);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("deleteContentTemplate")]
    public async Task<IActionResult> DeleteContentTemplate([FromBody] CContentTemplate contentTemplate)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      bool isDeleted = oReadWriteData.DeleteContentTemplate(contentTemplate);

      if (!isDeleted)
      {
        return BadRequest();
      }
      return Ok(isDeleted);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("deleteContentTemplateLink")]
    public async Task<IActionResult> DeleteContentTemplateLink([FromBody] CContentTemplateLink contentTemplateLink)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      bool isDeleted = oReadWriteData.DeleteContentTemplateLink(contentTemplateLink);

      if (!isDeleted)
      {
        return BadRequest();
      }
      return Ok(isDeleted);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("deleteContentTemplateImage")]
    public async Task<IActionResult> DeleteContentTemplateImage([FromBody] CContentTemplateImage contentTemplateImage)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CReadWriteData oReadWriteData = new CReadWriteData();
      bool isDeleted = oReadWriteData.DeleteContentTemplateImage(contentTemplateImage);

      if (!isDeleted)
      {
        return BadRequest();
      }
      return Ok(isDeleted);
    }

    /// <summary>
    /// Upload der Dateien für die Templates
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("uploadContentTemplateImage/{id}"), DisableRequestSizeLimit]
    public IActionResult UploadContentTemplateImage(int id)
    {
      try
      {
        var file = Request.Form.Files[0];
        var folderName = Path.Combine("resources", "images");
        

        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var originalName = fileName;
          var extention = Path.GetExtension(fileName);

          ImagesHelper imagesHelper = new ImagesHelper();
          CReadWriteData oReadWriteData = new CReadWriteData();
          CContentTemplateImage image = oReadWriteData.GetContendTemplateImageById(id);
          CContentTemplate contentTemplate = oReadWriteData.GetContendTemplatesById(image.ContentTemplateId);
          
          folderName = (new ImagesHelper()).GetFolderName(folderName, (CContentTemplate.ETemplateTypes)contentTemplate.Type);
          var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

          string strImagePathOriginal = Path.Combine(pathToSave, CImages.EFileDirectory.original.ToString());
          string strImagePathLarge = Path.Combine(pathToSave, CImages.EFileDirectory.large.ToString());
          string strImageFileSmall = Path.Combine(pathToSave, CImages.EFileDirectory.small.ToString());
          string strImagePathThumb = Path.Combine(pathToSave, CImages.EFileDirectory.thumb.ToString());

          imagesHelper.CreateDirectories(pathToSave, strImagePathOriginal, strImagePathLarge, strImageFileSmall, strImagePathThumb);

          fileName = Guid.NewGuid().ToString() + extention; //  fileName.Replace("(", "_");

          strImagePathOriginal = Path.Combine(strImagePathOriginal, fileName);
          strImagePathLarge = Path.Combine(strImagePathLarge, fileName);
          strImageFileSmall = Path.Combine(strImageFileSmall, fileName);
          strImagePathThumb = Path.Combine(strImagePathThumb, fileName);

          using (var stream = new FileStream(strImagePathOriginal, FileMode.Create))
          {
            file.CopyTo(stream);
            image.ImageOriginalName = originalName;
            image.ImageName = fileName;
            oReadWriteData.UpdateContentTemplateImage(image);
          }

          imagesHelper.Resize(strImagePathOriginal, strImagePathLarge, strImageFileSmall, strImagePathThumb);
          
          return Ok(new { image });
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

    //// PUT: api/Employees/5
    //[HttpPut("{id}")]
    //public async Task<IActionResult> PutRemark([FromRoute] int id, [FromBody] CRemark remark)
    //{
    //  if (!ModelState.IsValid)
    //  {
    //    return BadRequest(ModelState);
    //  }

    //  //CReadWriteData oReadWriteData = new CReadWriteData();
    //  //CRemark updatePerson = oReadWriteData.UpdateRem n(person, new CSettings());

    //  //if (id != updatePerson.ID)
    //  //{
    //  //  return BadRequest();
    //  //}
    //  //return Ok(updatePerson);

    //  return BadRequest();
    //}

    //// DELETE: api/ApiWithActions/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
  }
}
