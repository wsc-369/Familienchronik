using System;
using System.Collections.Generic;
using static appAhnenforschungBackEnd.DataManager.CGlobal;

namespace ValueObject
{
  public class ContentTemplate : ValueObject
  {
    public new Guid Id { get; set; }
    public int? RefContentTemplateId { get; set; }
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Content { get; set; }
    public int SortNo { get; set; }
    public TemplateTypes Type { get; set; }
    public  bool Active { get; set; }
    public List<ContentTemplateLink> ContentTemplateLinks { get; set; } = new List<ContentTemplateLink>();
    public List<ContentTemplateImage> ContentTemplateImages { get; set; } = new List<ContentTemplateImage>();

    
    protected override IEnumerable<object> GetEqualityComponents()
    {
      throw new NotImplementedException();
    }
  }
}
