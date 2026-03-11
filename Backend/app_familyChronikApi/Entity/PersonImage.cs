using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entity
{
  public class PersonImage : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string PersonRefId { get; set; }
    public string ImageName { get; set; }
    public string ImageOriginalSizePath { get; set; }
    public string ImageRootPath { get; set; }
    public string ImageSmallSizePath { get; set; }
    public string ImageBigSizePath { get; set; }

    public int ThemaType { get; set; }
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }
  }
}
