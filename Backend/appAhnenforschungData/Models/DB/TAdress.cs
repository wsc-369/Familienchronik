using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TAdress
    {
        public int NAdressId { get; set; }
        public string StrPersonId { get; set; }
        public string StrAdresse { get; set; }
        public string StrHouseNr { get; set; }
        public string StrTown { get; set; }
        public string StrZip { get; set; }
        public string StrCountry { get; set; }
        public int? NOrderNr { get; set; }
        public DateTime? DtUpdate { get; set; }
        public DateTime? DtCreate { get; set; }
        public bool? BActive { get; set; }
    }
}
