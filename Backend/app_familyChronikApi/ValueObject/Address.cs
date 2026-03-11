using System;
using System.Collections.Generic;

namespace ValueObject
{
  public class Address : ValueObject
  {
    public new Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string PersonRefId { get; set; }
    public string Street { get; set; }
    public string HouseNr { get; set; }
    public string Town { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }
    public string FullAddress { get; set; }
    public int OrderNr { get; set; }
    public string Description { get; set; }
    public  bool Active { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return PersonId;
      yield return Street;
      yield return HouseNr;
      yield return Town;
      yield return Zip;
      yield return FullAddress;
      yield return OrderNr;

      //yield return Familyname;
      //yield return FirstName;
      //yield return Bur;
      //yield return Status;
      //yield return BirthPlace;
      //yield return DeathPlace;
      //yield return BurPlace;
    }
  }
}
