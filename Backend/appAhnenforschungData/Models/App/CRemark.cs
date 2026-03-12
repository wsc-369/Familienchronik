using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()] 
  public class CRemark
  {
    public virtual string PersonID { get; set; }
    public string Remarks { get; set; }
    public string RemarksClean { get; set; }
    public bool Active { get; set; }
  }
}
