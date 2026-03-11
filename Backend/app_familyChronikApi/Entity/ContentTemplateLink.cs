using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
  public class ContentTemplateLink : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public Guid ContentTemplateId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public bool IsExternalLink { get; set; }
    public string NavigationTo { get; set; }
    public Guid PersonId { get; set; }
    public string Description { get; set; }
    public int SortNo { get; set; }
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
