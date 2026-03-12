using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TObituary
    {
        public int NObituaryId { get; set; }
        public string StrPersonId { get; set; }
        public string StrObituary { get; set; }
        public bool BActiv { get; set; }
    }
}
