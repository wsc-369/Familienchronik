using System;
using System.Collections.Generic;

namespace ValueObject
{
  public class ContentTemplateImage : ValueObject
  {
    public new Guid Id { get; set; }
    public Guid ContentTemplateId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string ImageName { get; set; }
    public string ImageOriginalName { get; set; }
    public string Description { get; set; }
    public int SortNo { get; set; }
    public bool Active { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
      throw new System.NotImplementedException();
    }
  }
}
