using System;
using System.Collections.Generic;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()] 
  public class CKinshipConnection
  {
    public int KinshipConnectionId { get; set; }
    public string ParentId { get; set; }
    public string PersonId { get; set; }
  }
}
