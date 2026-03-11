using Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
  public class MediaLibraryDocument : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string SourceFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public DateTime UploadDate { get; set; }

    [Required]
    public string ExtractedText { get; set; } = string.Empty;

    public required string Keywords { get; set; }

    public string KeywordsJson { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string FormatedHtml { get; set; } = string.Empty;

    public new bool Active { get; set; }

    public new DateTime AddTimestamp { get; set; }
    
    public new DateTime UpdateTimestamp { get; set; }
    
    public ContentTemplateLink ContentTemplateLink { get; set; } = new ContentTemplateLink();

    public ICollection<DocumentTopic> DocumentTopics { get; set; } = new List<DocumentTopic>();

  }
}
