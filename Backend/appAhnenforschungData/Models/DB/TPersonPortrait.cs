using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TPersonPortrait
    {
        public int NPersonPortraitId { get; set; }
        public string StrPersonId { get; set; }
        public string StrTitle { get; set; }
        public string StrPdfName { get; set; }
        public string StrRemarks { get; set; }
        public DateTime? DtUpdate { get; set; }
        public DateTime? DtCreate { get; set; }
        public int? NActive { get; set; }
    }
}
