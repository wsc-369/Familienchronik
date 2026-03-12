using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class VPartner
    {
        public string StrPersonId { get; set; }
        public string StrPartnerId { get; set; }
        public string TikMarriageDate { get; set; }
        public string TikDivorceDate { get; set; }
        public int? NCurrent { get; set; }
        public string StrName { get; set; }
        public string StrPreName { get; set; }
        public string StrFullname { get; set; }
        public string TikBirth { get; set; }
        public string TkDeath { get; set; }
        public string StrSex { get; set; }
        public string StrFatherId { get; set; }
        public string StrMotherId { get; set; }
        public string StrWork { get; set; }
        public string StrRace { get; set; }
        public string StrNick { get; set; }
        public int? NHasParents { get; set; }
        public int? NHasSpouse { get; set; }
        public int? NIsLiving { get; set; }
        public string StrMarriedName { get; set; }
        public string StrAdress { get; set; }
        public int? NBirthYear { get; set; }
        public int? NDeathYear { get; set; }
    }
}
