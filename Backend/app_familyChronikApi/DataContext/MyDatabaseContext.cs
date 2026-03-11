namespace app_familyBackend.DataContext
{
  using Entity;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Configuration;

  public class MyDatabaseContext : DbContext
  {
    public DbSet<ParentRole> ParentRoles { get; set; }

    public DbSet<GenderStatusOfPerson> PersonGenderStatuses { get; set; }

    public DbSet<Person> Persons { get; set; }

    public DbSet<Partner> Partners { get; set; }

    public DbSet<ParentChild> ParentChilds { get; set; }

    public DbSet<PartnerConnectionRole> PartnerConnectionRoles { get; set; }

    public DbSet<PersonRelation> PersonRelations { get; set; }

    public DbSet<PersonDetail> PersonDetails { get; set; }

    public DbSet<PersonImage> PersonImages { get; set; }

    public DbSet<PersonObituary> PersonObituaries { get; set; }

    public DbSet<PersonPortrait> PersonPortraits { get; set; }

    public DbSet<DialectWord> DialectWordCollection { get; set; }

    public DbSet<Address> Addresses { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<ImagePerson> ImagePersons { get; set; }

    public DbSet<ImagePersonPosition> ImagePersonPositions { get; set; }

    public DbSet<HistoryImagePersonPosition> HistoryImagePersonPositions { get; set; }

    public DbSet<KinshipConnection> KinshipConnections { get; set; }

    public DbSet<ContentTemplate> ContentTemplates { get; set; }

    public DbSet<ContentTemplateImage> ContentTemplateImages { get; set; }

    public DbSet<ContentTemplateLink> ContentTemplateLinks { get; set; }

    public DbSet<Family> Families { get; set; }

    public DbSet<MediaLibraryDocument> MediaLibraryDocuments { get; set; }

    public DbSet<Topic> Topics { get; set; }

    public DbSet<DocumentTopic> DocumentTopics { get; set; }


    private readonly IConfiguration _configuration;
    public MyDatabaseContext(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer(@"Server=your_server_name\instance_name;Database=your_database_name;User Id=your_username;Password=your_password;");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder = optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ChronikDateConnection"));
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.Entity<ParentChild>().HasOne(pc => pc.Parent).WithMany().OnDelete(DeleteBehavior.Restrict);
      modelBuilder.Entity<ParentChild>().HasOne(pc => pc.Child).WithMany().OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<DocumentTopic>()
              .HasKey(dt => new { dt.DocumentId, dt.TopicId });

      modelBuilder.Entity<DocumentTopic>()
          .HasOne(dt => dt.Document)
          .WithMany(d => d.DocumentTopics)
          .HasForeignKey(dt => dt.DocumentId);

      modelBuilder.Entity<DocumentTopic>()
          .HasOne(dt => dt.Topic)
          .WithMany(t => t.DocumentTopics)
          .HasForeignKey(dt => dt.TopicId);

    }
  }
}
