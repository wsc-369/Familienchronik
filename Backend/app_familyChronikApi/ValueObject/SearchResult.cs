using System.Reflection.Metadata;

namespace ValueObject
{

  public class SearchResult : ValueObject
  {
    public List<MediaLibraryDocument> Results { get; set; } = new List<MediaLibraryDocument>();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      throw new NotImplementedException();
    }
  }
}
