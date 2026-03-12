using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TPartner
    {
        public int NPartnerId { get; set; }
        public int? NPersonId { get; set; }
        public string StrPersonId { get; set; }
        public string StrPartnerId { get; set; }
        public string TikMarriageDate { get; set; }
        public string TikDivorceDate { get; set; }
        public string StrFullName { get; set; }
        public DateTime? DtMarriageDate { get; set; }
        public DateTime? DtDivorceDate { get; set; }
        public int? NCurrent { get; set; }
        public string StrCurrentFullName { get; set; }
        public int nConnectionType { get; set; }

    }
}
