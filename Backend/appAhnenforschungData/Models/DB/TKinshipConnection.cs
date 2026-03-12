using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TKinshipConnection
    {
        public int NKinshipConnectionId { get; set; }
        public string StrParentId { get; set; }
        public string StrPersonId { get; set; }
    }
}
