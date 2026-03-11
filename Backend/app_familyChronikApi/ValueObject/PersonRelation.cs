using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ValueObject
{
  public class PersonRelation : ValueObject
  {

    public new Guid BasePersonId { get; set; }

    public Person Person { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      throw new System.NotImplementedException();
    }
  }
}
