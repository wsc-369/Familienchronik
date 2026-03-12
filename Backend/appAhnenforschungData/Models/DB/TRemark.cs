using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TRemark
    {
        public int NRemarkId { get; set; }
        public string StrPersonId { get; set; }
        public string StrRemarks { get; set; }
        public bool? BActiv { get; set; }
        public string StrRemarksClean { get; set; }
    }
}
