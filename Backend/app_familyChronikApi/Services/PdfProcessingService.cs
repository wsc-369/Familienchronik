using app_familyBackend.DataContext;
using app_familyBackend.PdfExtractor;
using Entity;

namespace app_familyBackend.Services
{
  public class PdfProcessingService
  {
    // private readonly PdfTextExtractor _textExtractor;
    private readonly MyDatabaseContext _context;

    public PdfProcessingService(PdfTextExtractor textExtractor, MyDatabaseContext context)
    {
      // _textExtractor = textExtractor;
      _context = context;
    }

    public async Task<MediaLibraryDocument> ProcessPdfFile(string filePath, string fileName, MediaLibraryDocument entityDocument, string description = "")
    {
      try
      {
        string extractedText = PdfTextExtractor.ExtractTextFromPdf(filePath);
        string keywords = PdfTextExtractor.ExtractKeywords(extractedText, 255);
        string summary = PdfTextExtractor.GenerateSummary(extractedText);

        entityDocument.ExtractedText = extractedText;
        entityDocument.Keywords = keywords;
        entityDocument.Summary = summary;
  
        _context.MediaLibraryDocuments.Add(entityDocument);
        await _context.SaveChangesAsync();

        return entityDocument;
      }
      catch (Exception ex)
      {
        throw new Exception($"PDF-Verarbeitung fehlgeschlagen: {ex.Message}", ex);
      }
    }

    public async Task<MediaLibraryDocument> ProcessFreeText(Entity.MediaLibraryDocument entityDocument)
    {
      try
      {
        string extractedText = PdfTextExtractor.ExtractText(entityDocument.ExtractedText);
        string keywords = PdfTextExtractor.ExtractKeywords(extractedText, 255);
        string summary = PdfTextExtractor.GenerateSummary(extractedText);

        entityDocument.ExtractedText = extractedText;
        entityDocument.Keywords = keywords;
        entityDocument.Summary = summary;

        _context.MediaLibraryDocuments.Update(entityDocument);
        await _context.SaveChangesAsync();

        return entityDocument;
      }
      catch (Exception ex)
      {
        throw new Exception($"Process Freetext-Verarbeitung fehlgeschlagen: {ex.Message}", ex);
      }
    }
  }
}
