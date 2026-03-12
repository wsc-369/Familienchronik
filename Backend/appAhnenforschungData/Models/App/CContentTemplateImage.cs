using System;
using System.Collections.Generic;
using System.Text;

namespace appAhnenforschungData.Models.App
{

  [Serializable()]
  public class CContentTemplateImage
  {
    public int ContentTemplateImageId { get; set; }
    public int ContentTemplateId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string ImageName { get; set; }
    public string ImageOriginalName { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public int SortNo { get; set; }
  }
}
