using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ValueObject; // Ensure correct namespace usage if needed

namespace ValueObject
{
  // If ValueObject is a class, ensure it's referenced correctly.
  // If ValueObject is a namespace, you need to reference the actual class inside it.
  // For example, if the class is app_familyBackend.ValueObject.ValueObject, use:
  // public class Family : ValueObject

  // If ValueObject is not a class, you need to provide the correct base class.
  // If you have a ValueObject class, ensure it's defined and accessible.

  public class Family : ValueObject
  {

    [Key]
    public new Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string Tree { get; set; }
    public bool Active { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Id;
      yield return PersonId;
      yield return Tree;
      yield return Active;
    }
  }
}
