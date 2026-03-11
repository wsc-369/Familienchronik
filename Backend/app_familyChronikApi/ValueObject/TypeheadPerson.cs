using System;
using System.Collections.Generic;
namespace ValueObject
{
  public class TypeheadPerson : ValueObject
  {

    public TypeheadPerson(Guid personid, string familyname, string strFistName, DateTime birthDate)
    {
      Id = personid;
      FamilyName = familyname;
      FirstName = strFistName;
      BirthDate = birthDate;
    }

    public string FamilyName { get; set; } 

    public string FirstName { get; set; }

    public DateTime BirthDate { get; set; }


    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Id;
      yield return FamilyName;
      yield return FirstName;
      yield return BirthDate;
    }
  }
}