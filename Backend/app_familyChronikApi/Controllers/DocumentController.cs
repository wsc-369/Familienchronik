using app_familyBackend.DataContext;
using app_familyBackend.Services;
using app_familyChronikApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app_familyChronikApi.ReadWriteDB;

namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DocumentController : ControllerBase
  {
    private readonly PdfProcessingService _pdfService;
    private readonly MyDatabaseContext _context;
    private readonly ReadWirteContents _readWirteContents;
    private readonly SearchService _searchService;
    private readonly ILogger<DocumentController> _logger;

    public DocumentController(PdfProcessingService pdfService, MyDatabaseContext context, ReadWirteContents readWirteContents, SearchService searchService, ILogger<DocumentController> logger)
    {
      _logger = logger;
      _pdfService = pdfService;
      _context = context;
      _readWirteContents = readWirteContents;
      _searchService = searchService;
    }

    [HttpGet("SearchDocuments")]
    public async Task<IActionResult> SearchDocuments([FromQuery] string searchTerm = "", [FromQuery] int page = 0)
    {
      searchTerm ??= "";
      var results = await _searchService.SearchDocumentsAsync(searchTerm, page);
      return Ok(results);
    }


    [HttpGet("GetSearchSuggestions")] 
    public async Task<IActionResult> GetSearchSuggestions([FromQuery] string term) { 
      if (string.IsNullOrWhiteSpace(term) || term.Length < 2) 
        return Ok(new List<string>()); 
      var suggestions = _searchService.GetSearchSuggestionsAsync(term); 
      return Ok(suggestions); 
    }


    [HttpPost("MediaLibraryPdf/{id}"), DisableRequestSizeLimit]
    public async Task<IActionResult> MediaLibraryPdf(string id)
    {
      try
      {
        Guid _id = Guid.Parse(id);

        var file = Request.Form.Files[0];
        if (file == null || file.Length == 0)
          return BadRequest("No file uploaded.");


        var originalFolderName = Path.Combine("resources", "documents", "MediaLibraryDocuments", "Originals");
        var originaUploadsPath = Path.Combine(Directory.GetCurrentDirectory(), originalFolderName);
        if (!Directory.Exists(originaUploadsPath))
        {
          Directory.CreateDirectory(originaUploadsPath);
        }
        // Speichere die original Datei
        var fileSourceName = file.FileName;
        var fileSourcePath = Path.Combine(originaUploadsPath, fileSourceName);
        using (var stream = new FileStream(fileSourcePath, FileMode.Create))
        {
          await file.CopyToAsync(stream);
        }



        var folderSourceName = Path.Combine("resources", "documents", "MediaLibraryDocuments");
        var uploadsSaourcePath = Path.Combine(Directory.GetCurrentDirectory(), folderSourceName);
        if (!Directory.Exists(uploadsSaourcePath))
        {
          Directory.CreateDirectory(uploadsSaourcePath);
        }
        var guidfileName = $"{Guid.NewGuid()}_{file.FileName}";
        var guidFilePath = Path.Combine(uploadsSaourcePath, guidfileName);

        using (var stream = new FileStream(guidFilePath, FileMode.Create))
        {
          await file.CopyToAsync(stream);
        }

        var entityDocument = new Entity.MediaLibraryDocument { Keywords = "," };
        var contentTemplateLink = await this._context.ContentTemplateLinks.FirstOrDefaultAsync(ct => ct.Id == _id);
        entityDocument.Title = contentTemplateLink != null ? contentTemplateLink.Title : string.Empty;
        entityDocument.FileName = guidfileName;
        entityDocument.SourceFileName = fileSourceName;
        entityDocument.ExtractedText = "";
        entityDocument.FilePath = Path.Combine(folderSourceName, guidfileName);
        entityDocument.ContentType = file.ContentType;
        entityDocument.ContentTemplateLink = contentTemplateLink;
        entityDocument.Description = contentTemplateLink != null ? contentTemplateLink.SubTitle : string.Empty;
        entityDocument.FormatedHtml = "";
  
        var documentEntity = await _pdfService.ProcessPdfFile(entityDocument.FilePath, guidfileName, entityDocument);

        var document = await this._context.MediaLibraryDocuments.FirstOrDefaultAsync(ct => ct.Id == entityDocument.Id);

        return Ok(document);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", $"Fehler beim Hochladen: {ex.Message}");
        return StatusCode(500, ex.Message);
      }

    }

    /// <summary>
    /// Extracts and processes free text from a media library document by its ID and returns the updated document
    /// content.
    /// </summary>
    /// <param name="id">The unique identifier of the media library document.</param>
    /// <returns>An IActionResult containing the updated document content, or an error response if the operation fails.</returns>
    [HttpPost("MediaLibraryExtractText/{id}")]
    public async Task<IActionResult> MediaLibraryExtractText(string id)
    {
      try
      {
        if (!Guid.TryParse(id, out Guid _id))
          return BadRequest("Ungültige ID.");

        var entity = await _context.MediaLibraryDocuments.FirstOrDefaultAsync(ct => ct.Id == _id);

        if (entity == null)
          return NotFound("Template wurde nicht gefunden.");

        await _pdfService.ProcessFreeText(entity);

        var document = _readWirteContents.GetSingleContentTemplate(entity.Id);

        return Ok(document);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", $"Fehler beim Hochladen: {ex.Message}");
        return StatusCode(500, ex.Message);
      }
    }

    //[HttpPost]
    //public IActionResult Search(string searchTerm)
    //{
    //  if (string.IsNullOrWhiteSpace(searchTerm))
    //  {
    //    return RedirectToAction("Index");
    //  }

    //  // Volltextsuche in der Datenbank
    //  var results = _context.MediaLibraryDocuments
    //      .Where(d => d.ExtractedText.Contains(searchTerm) ||
    //                 d.Title.Contains(searchTerm) ||
    //                 d.Description.Contains(searchTerm))
    //      .ToList();

    //  //ViewBag.SearchTerm = searchTerm;
    //  //return View("Index", results);
    //  return Ok(results);
    //}
  }
}
