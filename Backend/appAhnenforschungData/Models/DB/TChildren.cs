using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TChildren
    {
        public int NChildrenId { get; set; }
        public string StrPersonId { get; set; }
        public string StrFatherId { get; set; }
        public string StrMotherId { get; set; }
        public string StrFullName { get; set; }
    }
}
