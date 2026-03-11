using System;
using System.Collections.Generic;
using ValueObject;

namespace ValueObject
{
  public class ContentTemplateLink : ValueObject
  {
    public new Guid Id { get; set; }
    public Guid ContentTemplateId { get; set; }
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public bool IsExternalLink { get; set; }
    public string? NavigationTo { get; set; }
    public Guid? PersonId { get; set; }
    public string? Description { get; set; }
    public int SortNo { get; set; } = 0;
    public bool Active { get; set; }  = false;

    public List<MediaLibraryDocument> MediaLibraryDocuments { get; set; } = new List<MediaLibraryDocument>();

    protected override IEnumerable<object> GetEqualityComponents()
    {
      throw new NotImplementedException();
    }
    //public DateTime AddTimestamp { get; set; }
    //public DateTime UpdateTimestamp { get; set; }
  }
}
