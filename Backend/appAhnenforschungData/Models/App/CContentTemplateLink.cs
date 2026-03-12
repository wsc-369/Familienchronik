using System;
using System.Collections.Generic;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()]
  public class CContentTemplateLink
  {
    public int ContentTemplateLinkId { get; set; }
    public int ContentTemplateId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public bool Active { get; set; }
    public bool ExternalLink { get; set; }
    public string NavigationTo { get; set; }
    public string PersonId { get; set; }
    public string Description { get; set; }
    public int SortNo { get; set; }
  }
}
