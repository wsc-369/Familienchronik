using System;
using System.Collections.Generic;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()]
  public class CSendEmail
  {
    public string EmailFrom { get; set; }
    public List<string> EmailTo { get; set; }
    public string Body { get; set; }
  }
}
