using Entity;

namespace Entity
{
  public class DocumentTopic : DomainModel
  {
    public Guid DocumentId { get; set; }
    public MediaLibraryDocument Document { get; set; }= new MediaLibraryDocument { Keywords = "," };

    public Guid TopicId { get; set; }
    public Topic Topic { get; set; } = new Topic();
  }
}
