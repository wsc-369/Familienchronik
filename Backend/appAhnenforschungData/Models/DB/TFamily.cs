using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TFamily
    {
        public int NFamilyId { get; set; }
        public string StrPersonId { get; set; }
        public string StrTree { get; set; }
        public DateTime? DtUpdate { get; set; }
        public DateTime? DtCreate { get; set; }
        public int? NActive { get; set; }
    }
}
