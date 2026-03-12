using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TPage
    {
        public int NPagesId { get; set; }
        public string StrHtml { get; set; }
        public string StrUser { get; set; }
        public DateTime? DtUpdateDate { get; set; }
        public int NPageNr { get; set; }
        public string StrDescription { get; set; }
        public bool BActive { get; set; }
    }
}
