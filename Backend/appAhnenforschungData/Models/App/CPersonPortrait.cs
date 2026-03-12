using System;
using System.Collections.Generic;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()]
  public class CPersonPortrait
  {
    public int PersonPortraitID { get; set; }
    public string PersonID { get; set; }
    public string Title { get; set; }
    public string PdfName { get; set; }
    public string Remarks { get; set; }
    public DateTime? Update { get; set; }
    public DateTime? Create { get; set; }
    public bool? Active { get; set; }
  }
}
