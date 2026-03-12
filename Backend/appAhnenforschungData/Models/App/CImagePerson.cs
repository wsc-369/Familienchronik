using appAhnenforschungData.Models.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace appAhnenforschungData.Models.App
{
    [Serializable()]
    public class CImagePerson
    {
        public int Id { get; set; }

        public string FileName { get; set; }

         public string OriginalFileName { get; set; }

        public string ImagePath { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [Display(Name = "Anzahl Personen")]
        [Range(0, 100)]
        public int PositionsCount { get; set; }

        public bool Active { get; set; }

         public bool InProgress { get; set; }

        public bool IsArchivated { get; set; }

        public bool IsExported { get; set; }

        public string SourceDescription { get; set; }

        public string SourceImageFileName { get; set; }

        public DateTime Add_Date { get; set; }

        public List<CImagePersonPosition> ImagePersonsPositions { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CImagePerson;

            if (other == null)
                return false;

            if (FileName != other.FileName
              || OriginalFileName != other.OriginalFileName
              || ImagePath != other.ImagePath
              || Title != other.Title
              || Description != other.Description
              || PositionsCount != other.PositionsCount
              || Active != other.Active
              || InProgress != other.InProgress
              || IsArchivated != other.IsArchivated
              || IsExported != other.IsExported
              || SourceDescription != other.SourceDescription
              || SourceImageFileName != other.SourceImageFileName
              || Add_Date != other.Add_Date)

                return false;

            return true;
        }

        public override int GetHashCode() { return 0; }
    }
}
