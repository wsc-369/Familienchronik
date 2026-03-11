
using System;
using System.Collections.Generic;
using static Entity.PartnerConnectionRole;

namespace ValueObject

{
  [Serializable()]
  public class Partner : ValueObject
  {
    public Partner(Guid partnerId, Guid personId)
    {
      PartnerId = partnerId;
      PersonId = personId;
    }
    public Guid PersonId { get; set; }

    public Guid PartnerId { get; set; }

    public Person Person { get; set; }

    public Person PartnerPerson { get; set; }

    public DateTime MarriageDateTime { get; set; }

    public DateTime DivorceDateTime { get; set; }

    public int ConnectionRole { get; set; } // Hier wird der Enum verwendet

    public bool IsCurrent;

    public bool Active;

    //public bool IsMarriage() {
    //  return MarriageDateTime != DateTime.MinValue;
    //}

    //public bool IsDivorce()
    //{
    //  return DivorceDateTime  != DateTime.MinValue;
    //}

    protected override IEnumerable<object> GetEqualityComponents()
    {
      throw new NotImplementedException();
    }
  }
}
