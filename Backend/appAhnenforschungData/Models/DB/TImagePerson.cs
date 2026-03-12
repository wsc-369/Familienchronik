using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TImagePerson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PositionsCount { get; set; }
        public bool? Active { get; set; }
        public bool? InProgress { get; set; }
        public DateTime? AddDate { get; set; }
        public string ImagePath { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public bool? IsArchivated { get; set; }
        public bool? IsExported { get; set; }
        public string SourceDescription { get; set; }
        public string SourceImageFileName { get; set; }
    }
}
