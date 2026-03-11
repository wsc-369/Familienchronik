using Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
  public class Topic : DomainModel
  {
    [Key]
    public new Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<DocumentTopic> DocumentTopics { get; set; } = new List<DocumentTopic>();

  }
}
