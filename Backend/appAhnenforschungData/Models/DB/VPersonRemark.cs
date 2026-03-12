using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class VPersonRemark
    {
        public int NPersonId { get; set; }
        public string StrPersonId { get; set; }
        public int? NRemarkId { get; set; }
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
        public int? NActive { get; set; }
        public int? NBirthDay { get; set; }
        public int? NBirthMonth { get; set; }
        public int? NBirthYear { get; set; }
        public int? NDeathDay { get; set; }
        public int? NDeathMonth { get; set; }
        public int? NDeathYear { get; set; }
        public string StrRemarks { get; set; }
        public bool? BActiv { get; set; }
        public string StrAdress { get; set; }
        public string StrEheName { get; set; }
    }
}
