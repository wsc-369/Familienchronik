namespace ValueObject
{
  public class ParentChild : ValueObject
  {
    public new Guid Id { get; set; }
    public bool Active { get; set; }
    public Person Child { get; set; }
    public Person Parent { get; set; }
    public int ParentRole { get; set; }
    public DateTime AddTimestamp { get; set; }
    public DateTime UpdateTimestamp { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      throw new NotImplementedException();
    }
  }
}
