using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class VParentsChildren
    {
        public string StrChildrenId { get; set; }
        public string StrChildrenFullName { get; set; }
        public string TikChildrenBirth { get; set; }
        public string TikChildrenDeath { get; set; }
        public string StrFatherId { get; set; }
        public string StrMotherId { get; set; }
        public string StrFatherFullName { get; set; }
        public string StrMotherFullName { get; set; }
        public string TikMarriageDate { get; set; }
        public string TikDivorceDate { get; set; }
    }
}
