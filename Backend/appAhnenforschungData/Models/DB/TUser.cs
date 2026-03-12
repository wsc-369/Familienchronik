using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TUser
    {
        public int NUserId { get; set; }
        public string StrPersonId { get; set; }
        public string StrSalutation { get; set; }
        public string StrLetter { get; set; }
        public string StrName { get; set; }
        public string StrPreName { get; set; }
        public string StrAdress { get; set; }
        public int NZip { get; set; }
        public string StrTown { get; set; }
        public string StrCountry { get; set; }
        public string StrEmail { get; set; }
        public string StrTel { get; set; }
        public string StrRemarks { get; set; }
        public int NRole { get; set; }
        public DateTime? DtAdmissionDate { get; set; }
        public DateTime? DtCheckOutDate { get; set; }
        public string StrLoginName { get; set; }
        public string StrPassword { get; set; }
        public bool? BHasPaid { get; set; }
        public DateTime? DtPaid { get; set; }
        public string StrPersonAccessList { get; set; }
        public bool BActive { get; set; }
        public bool BMustNotPaid { get; set; }
    }
}
