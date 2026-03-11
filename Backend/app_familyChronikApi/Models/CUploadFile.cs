using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appAhnenforschungBackEnd.Models
{
  [Serializable()] 
  public class CUploadFile
  {
    public string FileName { get; set; }
    public IFormFile File { get; set; }
    public int Size { get; set; }
    public string PathSource { get; set; }
    public string PathTarget { get; set; }
    public string ContentType { get; set; }
    public string PersonId { get; set; }
  }
}
