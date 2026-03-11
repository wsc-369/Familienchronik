using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Entity
{
  public class PersonRelation : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }
    [MaxLength]
    public string Value { get; set; } = string.Empty;
    public new bool Active { get; set; }
    public new DateTime AddTimestamp { get; set; }
    public new DateTime UpdateTimestamp { get; set; }


    //[NotMapped]
    //public ValueObject.Person Person
    //{
    //  get => JsonSerializer.Deserialize<Person>(Value) ?? new ValueObject.Person();
    //  set => Value = JsonSerializer.Serialize(value);
    //}
    [NotMapped]
    public ValueObject.Person Person
    {
      get => JsonSerializer.Deserialize<ValueObject.Person>(Value) ?? new ValueObject.Person();
      set => Value = JsonSerializer.Serialize(value, new JsonSerializerOptions
      {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
      });
    }
  }
}
