using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appAhnenforschungData.Models.App
{
    [Serializable()]
    public class CImagePersonProgress
    {
        public int Id { get; set; }
        public double Quantity { get; set; }
        public double QuantityReady { get; set; }
        public double Percent { get; set; }
        public string PercentDisplay { get; set; }
        public string ProgressClass { get; set; }
    }
}
