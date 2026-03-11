using System;
using System.ComponentModel.DataAnnotations;
using static appAhnenforschungBackEnd.DataManager.CGlobal;

namespace Entity
{
  public class ImagePerson : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }

    required public int RefImageId { get; set; }

    public Guid PersonId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int PositionsCount { get; set; }

    public ImagePersonState State { get; set; } // 0=IsActive,1=InProgress,2=IsArchivated,3=IsExported

    public string ImagePath { get; set; }

    public string FileName { get; set; }

    public string OriginalFileName { get; set; }

    public string SourceDescription { get; set; }

    public string SourceImageFileName { get; set; }

    public new DateTime AddTimestamp { get; set; }

    public new DateTime UpdateTimestamp { get; set; }
  }
}
