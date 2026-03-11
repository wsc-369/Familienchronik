using app_familyBackend.DataContext;
using app_familyChronikApi.ReadWriteDB;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using ValueObject;

namespace app_familyChronikApi.Services
{
  public class SearchService
  {
    private readonly MyDatabaseContext _context;
    private readonly ReadWirteContents _readWirteContents;

    public SearchService(MyDatabaseContext context, ReadWirteContents readWirteContents)
    {
      _context = context;
      _readWirteContents = readWirteContents;
    }

    public async Task<SearchResult> SearchDocumentsAsync(string searchTerm, int pageIndex = 0, int pageSize = 10)
    {
      var query = _context.MediaLibraryDocuments.AsQueryable();

      if (!string.IsNullOrWhiteSpace(searchTerm))
      {

        var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
          .Where(t => !StopWords.Contains(t))
          .ToArray();


        // Start mit einem Filter, der immer false ist
        Expression<Func<Entity.MediaLibraryDocument, bool>> filter = d => false;

        foreach (var term in terms)
        {
          var t = term; // wichtig für EF

          Expression<Func<Entity.MediaLibraryDocument, bool>> part = d =>
              d.Title.Contains(t) ||
              d.Description.Contains(t) ||
              d.ExtractedText.Contains(t) ||
              d.Keywords.Contains(t);

          filter = Or(filter, part);
        }

        if (filter != null)
        {
          query = query.Where(filter);
        }
      }

      var totalCount = await query.CountAsync();
      var documents = await query
          .OrderByDescending(d => d.UploadDate)
          .Skip(pageIndex * pageSize)
          .Take(pageSize)
          .ToListAsync();

      var mediaLibraryDocuments = new List<ValueObject.MediaLibraryDocument>();
      foreach (var doc in documents)
      {
        var obj = new MediaLibraryDocument();
        _readWirteContents.MapEntityToMediaLibraryDocument(doc, obj);
        mediaLibraryDocuments.Add(obj);
      }

      return new SearchResult
      {
        Results = mediaLibraryDocuments,
        TotalCount = totalCount,
        PageIndex = pageIndex,
        PageSize = pageSize
      };
    }

    private static Expression<Func<T, bool>> Or<T>(
        Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
      var parameter = Expression.Parameter(typeof(T));

      var body = Expression.OrElse(
          Expression.Invoke(expr1, parameter),
          Expression.Invoke(expr2, parameter));

      return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "der", "die", "das",
        "ein", "eine", "einer", "eines", "einem",
        "und", "oder", "aber",
        "im", "in", "am", "an", "auf", "zu", "vom", "von",
        "für", "mit", "ohne"
    };


    public async Task<SearchResult> SearchDocumentsAsyncOri(string searchTerm, int pageIndex = 0, int pageSize = 10)
    {
      var query = _context.MediaLibraryDocuments.AsQueryable();

      if (!string.IsNullOrWhiteSpace(searchTerm))
      {
        query = query.Where(d =>
            d.Title.Contains(searchTerm) ||
            d.Description.Contains(searchTerm) ||
            d.ExtractedText.Contains(searchTerm) ||
            d.Keywords.Contains(searchTerm));
      }

      var totalCount = await query.CountAsync();
      var documents = await query
          .OrderByDescending(d => d.UploadDate)
          .Skip(pageIndex * pageSize)
          .Take(pageSize)
          .ToListAsync();

      var mediaLibraryDocuments = new List<ValueObject.MediaLibraryDocument>();
      foreach (var doc in documents)
      {
        var obj = new MediaLibraryDocument();
        _readWirteContents.MapEntityToMediaLibraryDocument(doc, obj);
        mediaLibraryDocuments.Add(obj);
      }

      return new SearchResult
      {
        Results = mediaLibraryDocuments,
        TotalCount = totalCount,
        PageIndex = pageIndex,
        PageSize = pageSize
      };
    }

    public List<string> GetSearchSuggestionsAsync(string partialTerm)
    {
      if (string.IsNullOrWhiteSpace(partialTerm) || partialTerm.Length < 2)
        return new List<string>();

      partialTerm = partialTerm.ToLower();

      var allKeywords = _context.MediaLibraryDocuments
          .Where(d => !string.IsNullOrEmpty(d.Keywords))
          .AsEnumerable() // Wechsel zu LINQ-to-Objects
          .SelectMany(d => d.Keywords.Split(','))
          .Select(k => k.Trim())
          .Where(k => !string.IsNullOrEmpty(k) && k.ToLower().Contains(partialTerm))
          .Distinct()
          .Take(10)
          .ToList();

      return allKeywords;
    }
  }
}
