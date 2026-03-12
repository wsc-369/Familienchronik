using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()] 
  public class CObituary
  {
    public virtual string PersonID { get; set; }
    public string Obituary { get; set; }
    public bool Active { get; set; }
  }
}
