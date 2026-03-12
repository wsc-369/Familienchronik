using appAhnenforschungData.Models.App;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace appAhnenforschungData.Controllers
.Models.App
{
  [Serializable()] 
  public class CMasterData
  {


    public CPerson Person { get; set; }
    public List<CPartner> Partners { get; set; }
    public List<CPerson> Childrens { get; set; }
   
    [Display(Name = "Persönliches")]
    public CRemark Remarks { get; set; }
  }
}
