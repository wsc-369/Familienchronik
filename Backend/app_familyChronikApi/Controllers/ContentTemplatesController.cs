using app_familyBackend.DataContext;
using app_familyBackend.DataManager;
using app_familyBackend.Services;
using app_familyChronikApi.ReadWriteDB;
using appAhnenforschungBackEnd.Models;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    private readonly PdfProcessingService _pdfService;

    public ContentTemplatesController(MyDatabaseContext context, ReadWirteContents readWirteContents, PdfProcessingService pdfService, ILogger<ContentTemplatesController> logger)
    {
      _logger = logger;
      _context = context;
      _readWirteContents = readWirteContents;
      _pdfService = pdfService;
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


    // [Authorize(Roles = "Admin")]
    [HttpGet("getEmptyContentTemplate")]
    public async Task<ValueObject.ContentTemplate> getContentTemplate()
    {
      var template = await _readWirteContents.GetEmptyContentTemplate();


      return template;
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

    /// <summary>
    /// Creates a new content template
    /// </summary>
    //[Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ValueObject.ContentTemplate>> CreateContentTemplate(
        [FromBody] ValueObject.ContentTemplate contentTemplate)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var template = await _readWirteContents.AddContentTemplate(contentTemplate);
        if (contentTemplate.ContentTemplateImages != null)
        {
          foreach (var image in contentTemplate.ContentTemplateImages)
          {
            await _readWirteContents.AddOrUpdatetContentTemplateImage(image);
          }
        }
        foreach (var link in contentTemplate.ContentTemplateLinks)
        {
          await _readWirteContents.AddOrUpdatetContentTemplateLink(link);
          foreach (var media in link.MediaLibraryDocuments)
          {
            await _readWirteContents.AddOrUpdatetMediaLibraryDocument(media);
          }
        }

        if (template == null)
        {
          return BadRequest("Failed to create content template");
        }

        // REST-konform: 201 Created mit Location-Header
        return CreatedAtAction(
          nameof(getContentTemplate),
          new { id = template.Id },
          template
        );
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating content template");
        return StatusCode(500, "An error occurred while creating the content template");
      }
    }




    // [Authorize(Roles = "Admin")]
    [HttpPut("updateContentTemplate/{id}")]
    public async Task<IActionResult> UpdateContentTemplate(string id, [FromBody] ValueObject.ContentTemplate contentTemplate)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      //      var templateRelaton = await _readWirteContents.GetContendTemplatesById(contentTemplate.Id); ;
      //      var templateRelatonDeleete = await _readWirteContents.GetSingleContentTemplate(Guid.Parse(id));

      var template = await _readWirteContents.UpdateContentTemplate(contentTemplate);
      foreach (ValueObject.ContentTemplateLink contentTemplateLink in contentTemplate.ContentTemplateLinks)
      {
        await _readWirteContents.AddOrUpdatetContentTemplateLink(contentTemplateLink);

        foreach (ValueObject.MediaLibraryDocument mediaLibraryDocument in contentTemplateLink.MediaLibraryDocuments)
        {
          await _readWirteContents.AddOrUpdatetMediaLibraryDocument(mediaLibraryDocument);

        }

      }

      foreach (ValueObject.ContentTemplateImage contentTemplateImage in contentTemplate.ContentTemplateImages)
      {
        await _readWirteContents.AddOrUpdatetContentTemplateImage(contentTemplateImage);

      }


      if (template == null)
      {
        return NotFound();
      }
      return Ok(template);
    }

    /// <summary>
    /// Deletes a content template (soft delete - sets Active = false)
    /// </summary>
   // [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContentTemplate(string id)
    {
      if (!Guid.TryParse(id, out Guid guid))
      {
        return BadRequest("Invalid ID format");
      }

      try
      {
        var deleted = await _readWirteContents.DeleteContentTemplate(guid);

        if (!deleted)
        {
          return NotFound($"Content template with ID {id} not found");
        }

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting content template with ID: {Id}", id);
        return StatusCode(500, "An error occurred while deleting the content template");
      }
    }



    [HttpDelete("deleteContentTemplateLink/{id}")]
    public async Task<IActionResult> DeleteContentTemplateLink(string id)
    {
      if (!Guid.TryParse(id, out Guid guid))
      {
        return BadRequest("Invalid ID format");
      }

      try
      {
        var deleted = await _readWirteContents.DeleteContentTemplateLink(guid);

        if (!deleted)
        {
          return NotFound($"Content template with ID {id} not found");
        }

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting content template with ID: {Id}", id);
        return StatusCode(500, "An error occurred while deleting the content template");
      }
    }

    [HttpDelete("deleteContentTemplateImage/{id}")]
    public async Task<IActionResult> DeleteContentTemplateImage(string id)
    {
      if (!Guid.TryParse(id, out Guid guid))
      {
        return BadRequest("Invalid ID format");
      }

      try
      {
        var deleted = await _readWirteContents.DeleteContentTemplateImage(guid);

        if (!deleted)
        {
          return NotFound($"Content template with ID {id} not found");
        }

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting content template with ID: {Id}", id);
        return StatusCode(500, "An error occurred while deleting the content template");
      }
    }

    [HttpDelete("deleteContentMediaLibraryDocument/{id}")]
    public async Task<IActionResult> DeleteContentMediaLibraryDocument(string id)
    {
      if (!Guid.TryParse(id, out Guid guid))
      {
        return BadRequest("Invalid ID format");
      }

      try
      {
        var deleted = await _readWirteContents.DeleteContentMediaLibraryDocument(guid);

        if (!deleted)
        {
          return NotFound($"Content template with ID {id} not found");
        }

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting content template with ID: {Id}", id);
        return StatusCode(500, "An error occurred while deleting the content template");
      }
    }

    //[Authorize(Roles = "Admin")]
    //[HttpPost("deleteContentTemplateLink")]
    //public async Task<IActionResult> DeleteContentTemplateLink([FromBody] CContentTemplateLink contentTemplateLink)
    //{
    //  if (!ModelState.IsValid)
    //  {
    //    return BadRequest(ModelState);
    //  }

    //  CReadWriteData oReadWriteData = new CReadWriteData();
    //  bool isDeleted = oReadWriteData.DeleteContentTemplateLink(contentTemplateLink);

    //  if (!isDeleted)
    //  {
    //    return BadRequest();
    //  }
    //  return Ok(isDeleted);
    //}

    //[Authorize(Roles = "Admin")]
    //[HttpPost("deleteContentTemplateImage")]
    //public async Task<IActionResult> DeleteContentTemplateImage([FromBody] CContentTemplateImage contentTemplateImage)
    //{
    //  if (!ModelState.IsValid)
    //  {
    //    return BadRequest(ModelState);
    //  }

    //  CReadWriteData oReadWriteData = new CReadWriteData();
    //  bool isDeleted = oReadWriteData.DeleteContentTemplateImage(contentTemplateImage);

    //  if (!isDeleted)
    //  {
    //    return BadRequest();
    //  }
    //  return Ok(isDeleted);
    //}   

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

    /// <summary>
    /// Upload an image for a content template image
    /// </summary>
    //[Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("uploadImage"), DisableRequestSizeLimit]
    public async Task<IActionResult> UploadImage()
    {
      try
      {
        var file = Request.Form.Files.FirstOrDefault(f => f.Name == "file");
        var idString = Request.Form["id"].FirstOrDefault();

        if (file == null || string.IsNullOrEmpty(idString))
        {
          return BadRequest("File and ID are required");
        }

        if (!Guid.TryParse(idString, out Guid imageId))
        {
          return BadRequest("Invalid ID format");
        }

        if (file.Length == 0)
        {
          return BadRequest("File is empty");
        }

        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
        if (string.IsNullOrEmpty(fileName))
        {
          return BadRequest("Invalid file name");
        }

        var originalName = fileName;
        var extension = Path.GetExtension(fileName);
        var newFileName = Guid.NewGuid().ToString() + extension;

        // Get the image entity to determine the folder structure
        var image = await _readWirteContents.GetSingleContentTemplateImage(imageId);
        if (image == null || image.Id == Guid.Empty)
        {
          return NotFound($"Content template image with ID {imageId} not found");
        }

        var contentTemplate = await _readWirteContents.GetSingleContentTemplate(image.ContentTemplateId);
        if (contentTemplate == null || contentTemplate.Id == Guid.Empty)
        {
          return NotFound($"Content template not found");
        }

        // Create folder structure
        var folderName = Path.Combine("resources", "images");
        var imagesHelper = new ImagesHelper();
        // Convert TemplateTypes to ETemplateTypes by casting the integer value
        folderName = imagesHelper.GetFolderName(folderName, (CContentTemplate.ETemplateTypes)(int)contentTemplate.Type);
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        var strImagePathOriginal = Path.Combine(pathToSave, CImages.EFileDirectory.original.ToString());
        var strImagePathLarge = Path.Combine(pathToSave, CImages.EFileDirectory.large.ToString());
        var strImageFileSmall = Path.Combine(pathToSave, CImages.EFileDirectory.small.ToString());
        var strImagePathThumb = Path.Combine(pathToSave, CImages.EFileDirectory.thumb.ToString());

        imagesHelper.CreateDirectories(pathToSave, strImagePathOriginal, strImagePathLarge, strImageFileSmall, strImagePathThumb);

        // Save the original file
        var fullPathOriginal = Path.Combine(strImagePathOriginal, newFileName);
        var fullPathLarge = Path.Combine(strImagePathLarge, newFileName);
        var fullPathSmall = Path.Combine(strImageFileSmall, newFileName);
        var fullPathThumb = Path.Combine(strImagePathThumb, newFileName);

        using (var stream = new FileStream(fullPathOriginal, FileMode.Create))
        {
          await file.CopyToAsync(stream);
        }

        // Create resized versions
        imagesHelper.Resize(fullPathOriginal, fullPathLarge, fullPathSmall, fullPathThumb);

        // Update the image entity
        image.ImageOriginalName = originalName;
        image.ImageName = newFileName;
        await _readWirteContents.AddOrUpdatetContentTemplateImage(image);

        return Ok(new { imageName = newFileName });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error uploading image");
        return StatusCode(500, "An error occurred while uploading the image");
      }
    }

    /// <summary>
    /// Upload a document for a media library document
    /// </summary>
    //[Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("uploadDocument"), DisableRequestSizeLimit]
    public async Task<IActionResult> UploadDocument()
    {
      try
      {
        var file = Request.Form.Files.FirstOrDefault(f => f.Name == "file");
        var payloadJson = Request.Form["payload"].FirstOrDefault();

        if (file == null || string.IsNullOrEmpty(payloadJson))
        {
          return BadRequest("File and payload are required");
        }
        // Deserialize the payload JSON with case-insensitive option
        var options = new System.Text.Json.JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        };

        var payload = System.Text.Json.JsonSerializer.Deserialize<UploadDocumentPayload>(payloadJson, options);
        if (payload == null || payload.Id == Guid.Empty)
        {
          return BadRequest("Invalid payload or ID");
        }

        var idString = Request.Form["id"].FirstOrDefault();
        // var contentTemplateLinkId = payloadJson.FirstOrDefault(x=> x. Request.Form["contentTemplateLinkId"].FirstOrDefault();

        if (file == null || string.IsNullOrEmpty(idString))
        {
          return BadRequest("File and ID are required");
        }

        if (!Guid.TryParse(idString, out Guid documentId))
        {
          return BadRequest("Invalid ID format");
        }

        if (file.Length == 0)
        {
          return BadRequest("File is empty");
        }

        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
        if (string.IsNullOrEmpty(fileName))
        {
          return BadRequest("Invalid file name");
        }

        var originalName = fileName;
        var extension = Path.GetExtension(fileName);
        var newFileName = Guid.NewGuid().ToString() + extension;
        var contentType = file.ContentType;

        // Get the media library document
        var document = await _readWirteContents.GetSingleMediaLibraryDocument(documentId);
        if (document == null || document.Id == Guid.Empty)
        {
          return NotFound($"Media library document with ID {documentId} not found");
        }

        // Create folder structure for documents
        var folderName = Path.Combine("resources", "documents");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        // Ensure directory exists
        if (!Directory.Exists(pathToSave))
        {
          Directory.CreateDirectory(pathToSave);
        }

        // Save the file
        var fullPath = Path.Combine(pathToSave, newFileName);
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
          await file.CopyToAsync(stream);
        }

        // Update the document entity
        document.FilePath = Path.Combine(folderName, newFileName);
        document.ContentType = contentType;

        // Extract text if it's a PDF or text document
        string extractedText = string.Empty;
        if (contentType == "application/pdf" || contentType == "text/plain")
        {
          // TODO: Implement text extraction logic
          // You might want to use a library like iTextSharp or PdfPig for PDF extraction
          extractedText = "Text extraction not yet implemented";
        }
        document.ExtractedText = extractedText;

        await _readWirteContents.AddOrUpdatetMediaLibraryDocument(document);


        //document = await _readWirteContents.GetSingleMediaLibraryDocument(documentId);
        if (payload != null)
        {
          //if (!Guid.TryParse(payload, out Guid guid))
          //{
          //  return BadRequest("Invalid ID format");
          //}
          var entityDocument = new Entity.MediaLibraryDocument { Keywords = "," };
          var contentTemplateLink = await this._context.ContentTemplateLinks.FirstOrDefaultAsync(ct => ct.Id == payload.ContentTemplateLinkId);
          entityDocument.Title = contentTemplateLink != null ? contentTemplateLink.Title : string.Empty;
          entityDocument.FileName = newFileName;
          entityDocument.SourceFileName = originalName;
          entityDocument.ExtractedText = "";
          entityDocument.FilePath = Path.Combine(pathToSave, newFileName);
          entityDocument.ContentType = file.ContentType;
          entityDocument.ContentTemplateLink = contentTemplateLink;
          entityDocument.Description = contentTemplateLink != null ? contentTemplateLink.SubTitle : string.Empty;
          entityDocument.FormatedHtml = "";

          var documentEntity = await _pdfService.ProcessPdfFile(entityDocument.FilePath, newFileName, entityDocument);
        }
        return Ok(new
        {
          fileName = newFileName,
          originalFileName = originalName,
          filePath = document.FilePath,
          contentType = contentType,
          fileSize = file.Length,
          extractedText = extractedText
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error uploading document");
        return StatusCode(500, "An error occurred while uploading the document");
      }
    }

    // Add this payload class at the end of the controller or in a separate file
    public class UploadDocumentPayload
    {
      public Guid Id { get; set; }
      public Guid? ContentTemplateLinkId { get; set; }
      public Guid? ContentTemplateId { get; set; }
      public string? Title { get; set; }
      public string? Description { get; set; }
      public string? FilePath { get; set; }
      public string? ContentType { get; set; }
      public string? Keywords { get; set; }
      public string? KeywordsJson { get; set; }
      public string? Summary { get; set; }
      public string? ExtractedText { get; set; }
      public string? FormatedHtml { get; set; }
      public bool Active { get; set; }
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
