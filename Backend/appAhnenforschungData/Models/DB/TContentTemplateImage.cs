using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TContentTemplateImage
    {
        public int NContentTemplateImageId { get; set; }
        public int NContentTemplateId { get; set; }
        public string StrTitle { get; set; }
        public string StrSubTitle { get; set; }
        public string StrImageName { get; set; }
        public string StrDescription { get; set; }
        public int NActive { get; set; }
        public int? NSortNo { get; set; }
        public string StrImageOriginalName { get; set; }
    }
}
