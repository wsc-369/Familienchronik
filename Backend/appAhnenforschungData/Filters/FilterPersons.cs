using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appAhnenforschungBackEnd.Filters
{
  public class FilterPersons
  {
    public string personID { get; set; }
    public string familyName { get; set; }
    public string firstName { get; set; }
    public string birthDate { get; set; }
    public string deathDate { get; set; }
    public string birthYear { get; set; }
    public string older { get; set; }

        public string dateFrom { get; set; }
        public string dateUntil { get; set; }
        public string wildCardText { get; set; }

  }
}
