using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()] 
  public class CFam
  {
    public CPerson Parent { get; set; }
    public string InnerHtml { get; set; }
  }
}
