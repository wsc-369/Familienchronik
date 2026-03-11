using System;
using System.Collections.Generic;

namespace ValueObject
{
  public class PersonPortrait : ValueObject
  {

    public Guid PersonId { get; set; }
    public string Title { get; set; }
    public string PdfName { get; set; }
    public string Remarks { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public bool Active { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return PersonId;
      yield return Title;
      yield return PdfName;
      yield return Remarks;
      yield return CreateDate;
      yield return UpdateDate;
      yield return Active;
    }
  }
}
