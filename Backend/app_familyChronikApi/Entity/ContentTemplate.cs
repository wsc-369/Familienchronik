using System;
using System.ComponentModel.DataAnnotations;
using static appAhnenforschungBackEnd.DataManager.CGlobal;

namespace Entity
{
  public class ContentTemplate : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public int RefContentTemplateId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string Content { get; set; }
    public int SortNo { get; set; }
    public TemplateTypes Type { get; set; }
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
