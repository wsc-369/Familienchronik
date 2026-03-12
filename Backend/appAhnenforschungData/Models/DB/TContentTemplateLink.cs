using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TContentTemplateLink
    {
        public int NContentTemplateLinkId { get; set; }
        public int NContentTemplateId { get; set; }
        public string StrTitle { get; set; }
        public string StrSubTitle { get; set; }
        public int NActive { get; set; }
        public int ExternalLink { get; set; }
        public string StrNavigationTo { get; set; }
        public string StrPersonId { get; set; }
        public string StrDescription { get; set; }
        public int? NSortNo { get; set; }
    }
}
