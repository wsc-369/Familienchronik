using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class Tlemma
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public int Source { get; set; }
        public int Place { get; set; }
        public string Voice { get; set; }
        public string StrPersonId { get; set; }
  }
}
