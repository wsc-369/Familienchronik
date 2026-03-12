using System;
using System.Collections.Generic;
using System.Text;

namespace appAhnenforschungData.Models.App
{
    [Serializable()]
    public class CContentTemplate
    {
        public enum ETemplateTypes { undefind = -1, club = 10, restaurant = 20, theatricalLife = 30, shop = 40, excusion = 50, mediatek = 60, whoIsInThePhoto = 70, mainPageSlide = 80, clubAhnenforschung = 90, stammNamen = 100, themaOverview = 110 };

        public int ContentTemplateId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Content { get; set; }
        public bool Active { get; set; }
        public int Type { get; set; }

        public int SortNo { get; set; }

        public List<CContentTemplateLink> ContentTemplateLinks { get; set; }
        public List<CContentTemplateImage> ContentTemplateImages { get; set; }
    }
}
