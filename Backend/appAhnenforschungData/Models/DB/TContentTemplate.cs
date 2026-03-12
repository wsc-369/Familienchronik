using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TContentTemplate
    {
        public int NContentTemplateId { get; set; }
        public string StrTitle { get; set; }
        public string StrSubTitle { get; set; }
        public string StrContent { get; set; }
        public int NActive { get; set; }
        public int NType { get; set; }
        public int? NSortNo { get; set; }
    }
}
