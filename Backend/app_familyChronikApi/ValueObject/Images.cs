using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static appAhnenforschungBackEnd.DataManager.CGlobal;

namespace ValueObject
{
    [Serializable()]
    public class Images
    {

        // public enum EType { undefind = -1, person = 10, mainSlide = 20 };
        public enum EFileDirectory { original, large, small, thumb };

        public int index { get; set; }
        public string urlOriginal { get; set; }
        public string urlLarge { get; set; }
        public string urlSmall { get; set; }
        public string urlThumb { get; set; }
        public string title { get; set; }
        public string subTitle { get; set; }
        public string description { get; set; }
        public TemplateTypes type { get; set; }
        public string imageName { get; set; }
        public int sortNo { get; set; }
    }
}
