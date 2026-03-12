using Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ValueObject
{
  public class MediaLibraryDocument : ValueObject
  {
    public new Guid Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public string FilePath { get; set; }

    public string ContentType { get; set; }

    //public DateTime UploadDate { get; set; }

    public string ExtractedText { get; set; }

    public string Keywords { get; set; }

    public string KeywordsJson { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string FormatedHtml { get; set; }

    public bool Active { get; set; }

    public ContentTemplateLink ContentTemplateLink { get; set; } = new ContentTemplateLink();

 //   public ICollection<DocumentTopic> DocumentTopics { get; set; } = new List<DocumentTopic>(); // TODO: Es wird die Entity verwendet, da die ValueObject-Collection nicht funktioniert. Es muss noch geprüft werden, ob das so bleibt oder ob es eine andere Lösung gibt.
    //ContentTemplateLink { get; set; } = new ContentTemplateLink();

    // public ICollection<DocumentTopic> DocumentTopics { get; set; } = new List<DocumentTopic>();

    // public string CreatedBy { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      throw new NotImplementedException();
    }
  }
}
