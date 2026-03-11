using System;
using System.Collections.Generic;

namespace ValueObject
{
  public class DialectWord : ValueObject
  {
    public new Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Voice { get; set; }
    public string PersonFamilyName { get; set; } 
    public string PersonFirstname { get; set; }
    public bool Active { get; set; }

    //public int Place { get; set; }
    //public int Source { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return PersonId;
      yield return Title;
      yield return Description;
      yield return Voice;
      yield return PersonFamilyName;
      yield return PersonFirstname;
      yield return Active;
    }
  }
}
