using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Configuration;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
  public partial class wsc_chronikContext : DbContext
  {
    private readonly IConfiguration _configuration;

    public wsc_chronikContext()
    {
      
    }

    public wsc_chronikContext(DbContextOptions<wsc_chronikContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TAdress> TAdresses { get; set; }
    public virtual DbSet<TChildren> TChildrens { get; set; }
    public virtual DbSet<TContentTemplate> TContentTemplates { get; set; }
    public virtual DbSet<TContentTemplateImage> TContentTemplateImages { get; set; }
    public virtual DbSet<TContentTemplateLink> TContentTemplateLinks { get; set; }
    public virtual DbSet<TFamily> TFamilies { get; set; }
    public virtual DbSet<TImagePerson> TImagePersons { get; set; }
    public virtual DbSet<TImagePersonsPosition> TImagePersonsPositions { get; set; }
    public virtual DbSet<TImagePersonsPositionsHistory> TImagePersonsPositionsHistories { get; set; }
    public virtual DbSet<TKinshipConnection> TKinshipConnections { get; set; }
    public virtual DbSet<TObituary> TObituaries { get; set; }
    public virtual DbSet<TPage> TPages { get; set; }
    public virtual DbSet<TPartner> TPartners { get; set; }
    public virtual DbSet<TPerson> TPersons { get; set; }
    public virtual DbSet<TPersonPortrait> TPersonPortraits { get; set; }
    public virtual DbSet<TRemark> TRemarks { get; set; }
    public virtual DbSet<TUser> TUsers { get; set; }
    public virtual DbSet<Tlemma> Tlemmas { get; set; }
    public virtual DbSet<VParentsChildren> VParentsChildrens { get; set; }
    public virtual DbSet<VPartner> VPartners { get; set; }
    public virtual DbSet<VPersonAdress> VPersonAdresses { get; set; }
    public virtual DbSet<VPersonRemark> VPersonRemarks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
       optionsBuilder.UseSqlServer("Server=DESKTOP-39TVB67;Database=ahnen_chronik;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;");
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

      modelBuilder.Entity<TAdress>(entity =>
      {
        entity.HasKey(e => e.NAdressId)
                  .HasName("PK_tAdress");

        entity.ToTable("tAdresses");

        entity.Property(e => e.NAdressId).HasColumnName("nAdressID");

        entity.Property(e => e.BActive).HasColumnName("bActive");

        entity.Property(e => e.DtCreate)
                  .HasColumnType("datetime")
                  .HasColumnName("dtCreate");

        entity.Property(e => e.DtUpdate)
                  .HasColumnType("datetime")
                  .HasColumnName("dtUpdate");

        entity.Property(e => e.NOrderNr).HasColumnName("nOrderNr");

        entity.Property(e => e.StrAdresse)
                  .HasMaxLength(255)
                  .HasColumnName("strAdresse");

        entity.Property(e => e.StrCountry)
                  .HasMaxLength(50)
                  .HasColumnName("strCountry");

        entity.Property(e => e.StrHouseNr)
                  .HasMaxLength(10)
                  .HasColumnName("strHouseNr")
                  .IsFixedLength(true);

        entity.Property(e => e.StrPersonId)
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");

        entity.Property(e => e.StrTown)
                  .HasMaxLength(50)
                  .HasColumnName("strTown");

        entity.Property(e => e.StrZip)
                  .HasMaxLength(5)
                  .HasColumnName("strZip")
                  .IsFixedLength(true);
      });

      modelBuilder.Entity<TChildren>(entity =>
      {
        entity.HasKey(e => e.NChildrenId);

        entity.ToTable("tChildrens");

        entity.Property(e => e.NChildrenId).HasColumnName("nChildrenID");

        entity.Property(e => e.StrFatherId)
                  .HasMaxLength(8)
                  .HasColumnName("strFatherID");

        entity.Property(e => e.StrFullName)
                  .HasMaxLength(50)
                  .HasColumnName("strFullName");

        entity.Property(e => e.StrMotherId)
                  .HasMaxLength(8)
                  .HasColumnName("strMotherID");

        entity.Property(e => e.StrPersonId)
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");
      });

      modelBuilder.Entity<TContentTemplate>(entity =>
      {
        entity.HasKey(e => e.NContentTemplateId);

        entity.ToTable("tContentTemplates");

        entity.Property(e => e.NContentTemplateId).HasColumnName("nContentTemplateID");

        entity.Property(e => e.NActive).HasColumnName("nActive");

        entity.Property(e => e.NSortNo).HasColumnName("nSortNo");

        entity.Property(e => e.NType)
                  .HasColumnName("nType")
                  .HasDefaultValueSql("((-1))");

        entity.Property(e => e.StrContent)
                  .IsRequired()
                  .HasColumnName("strContent");

        entity.Property(e => e.StrSubTitle)
                  .IsRequired()
                  .HasMaxLength(255)
                  .HasColumnName("strSubTitle");

        entity.Property(e => e.StrTitle)
                  .IsRequired()
                  .HasMaxLength(255)
                  .HasColumnName("strTitle");
      });

      modelBuilder.Entity<TContentTemplateImage>(entity =>
      {
        entity.HasKey(e => e.NContentTemplateImageId);

        entity.ToTable("tContentTemplateImages");

        entity.Property(e => e.NContentTemplateImageId).HasColumnName("nContentTemplateImageID");

        entity.Property(e => e.NActive).HasColumnName("nActive");

        entity.Property(e => e.NContentTemplateId).HasColumnName("nContentTemplateID");

        entity.Property(e => e.NSortNo).HasColumnName("nSortNo");

        entity.Property(e => e.StrDescription).HasColumnName("strDescription");

        entity.Property(e => e.StrImageName)
                  .HasMaxLength(255)
                  .HasColumnName("strImageName");

        entity.Property(e => e.StrImageOriginalName)
                  .HasMaxLength(255)
                  .HasColumnName("strImageOriginalName")
                  .HasDefaultValueSql("('')");

        entity.Property(e => e.StrSubTitle)
                  .IsRequired()
                  .HasMaxLength(255)
                  .HasColumnName("strSubTitle");

        entity.Property(e => e.StrTitle)
                  .IsRequired()
                  .HasMaxLength(255)
                  .HasColumnName("strTitle");
      });

      modelBuilder.Entity<TContentTemplateLink>(entity =>
      {
        entity.HasKey(e => e.NContentTemplateLinkId);

        entity.ToTable("tContentTemplateLinks");

        entity.Property(e => e.NContentTemplateLinkId).HasColumnName("nContentTemplateLinkID");

        entity.Property(e => e.ExternalLink).HasColumnName("externalLink");

        entity.Property(e => e.NActive).HasColumnName("nActive");

        entity.Property(e => e.NContentTemplateId).HasColumnName("nContentTemplateID");

        entity.Property(e => e.NSortNo).HasColumnName("nSortNo");

        entity.Property(e => e.StrDescription).HasColumnName("strDescription");

        entity.Property(e => e.StrNavigationTo)
                  .HasMaxLength(255)
                  .HasColumnName("strNavigationTo");

        entity.Property(e => e.StrPersonId)
                  .IsRequired()
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");

        entity.Property(e => e.StrSubTitle)
                  .IsRequired()
                  .HasMaxLength(255)
                  .HasColumnName("strSubTitle");

        entity.Property(e => e.StrTitle)
                  .IsRequired()
                  .HasMaxLength(255)
                  .HasColumnName("strTitle");
      });

      modelBuilder.Entity<TFamily>(entity =>
      {
        entity.HasKey(e => e.NFamilyId);

        entity.ToTable("tFamily");

        entity.Property(e => e.NFamilyId).HasColumnName("nFamilyID");

        entity.Property(e => e.DtCreate)
                  .HasColumnType("datetime")
                  .HasColumnName("dtCreate");

        entity.Property(e => e.DtUpdate)
                  .HasColumnType("datetime")
                  .HasColumnName("dtUpdate");

        entity.Property(e => e.NActive).HasColumnName("nActive");

        entity.Property(e => e.StrPersonId)
                  .IsRequired()
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");

        entity.Property(e => e.StrTree).HasColumnName("strTree");
      });

      modelBuilder.Entity<TImagePerson>(entity =>
      {
        entity.ToTable("tImagePersons");

        entity.Property(e => e.AddDate)
                  .HasColumnType("datetime")
                  .HasColumnName("Add_Date");

        entity.Property(e => e.FileName).HasMaxLength(255);

        entity.Property(e => e.ImagePath)
                  .IsRequired()
                  .HasMaxLength(255);

        entity.Property(e => e.OriginalFileName).HasMaxLength(255);

        entity.Property(e => e.SourceDescription).HasMaxLength(255);

        entity.Property(e => e.SourceImageFileName).HasMaxLength(255);

        entity.Property(e => e.Title)
                  .IsRequired()
                  .HasMaxLength(255);
      });

      modelBuilder.Entity<TImagePersonsPosition>(entity =>
      {
        entity.ToTable("tImagePersonsPositions");

        entity.Property(e => e.AddDate)
                  .HasColumnType("datetime")
                  .HasColumnName("Add_Date");

        entity.Property(e => e.Address).HasMaxLength(255);

        entity.Property(e => e.Country).HasMaxLength(50);

        entity.Property(e => e.EditContactData).HasMaxLength(255);

        entity.Property(e => e.EditEmail).HasMaxLength(50);

        entity.Property(e => e.HouseNo).HasMaxLength(10);

        entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(50);

        entity.Property(e => e.PreName)
                  .IsRequired()
                  .HasMaxLength(50);

        entity.Property(e => e.ReferencePersonId).HasMaxLength(8);

        entity.Property(e => e.Town).HasMaxLength(50);

        entity.Property(e => e.UpdDate)
                  .HasColumnType("datetime")
                  .HasColumnName("Upd_Date");

        entity.Property(e => e.Zip).HasMaxLength(5);
      });

      modelBuilder.Entity<TImagePersonsPositionsHistory>(entity =>
      {
        entity.ToTable("tImagePersonsPositionsHistory");

        entity.Property(e => e.AddDate)
                  .HasColumnType("datetime")
                  .HasColumnName("Add_Date");

        entity.Property(e => e.AddHistoryDate)
                  .HasColumnType("datetime")
                  .HasColumnName("AddHistory_Date");

        entity.Property(e => e.Address).HasMaxLength(255);

        entity.Property(e => e.Country).HasMaxLength(50);

        entity.Property(e => e.EditContactData).HasMaxLength(255);

        entity.Property(e => e.EditEmail).HasMaxLength(50);

        entity.Property(e => e.HouseNo).HasMaxLength(10);

        entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(50);

        entity.Property(e => e.PreName)
                  .IsRequired()
                  .HasMaxLength(50);

        entity.Property(e => e.ReferencePersonId).HasMaxLength(8);

        entity.Property(e => e.Town).HasMaxLength(50);

        entity.Property(e => e.UpdDate)
                  .HasColumnType("datetime")
                  .HasColumnName("Upd_Date");

        entity.Property(e => e.Zip).HasMaxLength(5);
      });

      modelBuilder.Entity<TKinshipConnection>(entity =>
      {
        entity.HasKey(e => e.NKinshipConnectionId);

        entity.ToTable("tKinshipConnections");

        entity.Property(e => e.NKinshipConnectionId).HasColumnName("nKinshipConnectionID");

        entity.Property(e => e.StrParentId)
                  .IsRequired()
                  .HasMaxLength(8)
                  .HasColumnName("strParentID");

        entity.Property(e => e.StrPersonId)
                  .IsRequired()
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");
      });

      modelBuilder.Entity<TObituary>(entity =>
      {
        entity.HasKey(e => e.NObituaryId);

        entity.ToTable("tObituary");

        entity.Property(e => e.NObituaryId).HasColumnName("nObituaryID");

        entity.Property(e => e.BActiv).HasColumnName("bActiv");

        entity.Property(e => e.StrObituary)
                  .IsRequired()
                  .HasColumnName("strObituary");

        entity.Property(e => e.StrPersonId)
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");
      });

      modelBuilder.Entity<TPage>(entity =>
      {
        entity.HasKey(e => e.NPagesId);

        entity.ToTable("tPages");

        entity.Property(e => e.NPagesId).HasColumnName("nPagesID");

        entity.Property(e => e.BActive).HasColumnName("bActive");

        entity.Property(e => e.DtUpdateDate)
                  .HasColumnType("datetime")
                  .HasColumnName("dtUpdateDate");

        entity.Property(e => e.NPageNr).HasColumnName("nPageNr");

        entity.Property(e => e.StrDescription)
                  .HasMaxLength(250)
                  .HasColumnName("strDescription");

        entity.Property(e => e.StrHtml)
                  .IsRequired()
                  .HasColumnName("strHtml");

        entity.Property(e => e.StrUser)
                  .HasMaxLength(50)
                  .HasColumnName("strUser");
      });

      modelBuilder.Entity<TPartner>(entity =>
      {
        entity.HasKey(e => e.NPartnerId)
                  .HasName("PK_tPartner");

        entity.ToTable("tPartners");

        entity.Property(e => e.NPartnerId).HasColumnName("nPartnerID");

        entity.Property(e => e.DtDivorceDate)
                  .HasColumnType("date")
                  .HasColumnName("dtDivorceDate");

        entity.Property(e => e.DtMarriageDate)
                  .HasColumnType("date")
                  .HasColumnName("dtMarriageDate");

        entity.Property(e => e.NCurrent).HasColumnName("nCurrent");

        entity.Property(e => e.NPersonId).HasColumnName("nPersonID");

        entity.Property(e => e.StrCurrentFullName)
                  .HasMaxLength(50)
                  .HasColumnName("strCurrentFullName");

        entity.Property(e => e.StrFullName)
                  .HasMaxLength(50)
                  .HasColumnName("strFullName");

        entity.Property(e => e.StrPartnerId)
                  .HasMaxLength(8)
                  .HasColumnName("strPartnerID");

        entity.Property(e => e.StrPersonId)
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");

        entity.Property(e => e.TikDivorceDate)
                  .HasMaxLength(50)
                  .HasColumnName("tikDivorceDate");

        entity.Property(e => e.TikMarriageDate)
                  .HasMaxLength(50)
                  .HasColumnName("tikMarriageDate");
      });

      modelBuilder.Entity<TPerson>(entity =>
      {
        entity.HasKey(e => e.NPersonId);

        entity.ToTable("tPersons");

        entity.HasIndex(e => e.StrFatherId, "IX_tPersons_strFatherID");

        entity.HasIndex(e => e.StrMotherId, "IX_tPersons_strMotherID");

        entity.Property(e => e.NPersonId).HasColumnName("nPersonID");

        entity.Property(e => e.NActive).HasColumnName("nActive");

        entity.Property(e => e.NBirthDay).HasColumnName("nBirthDay");

        entity.Property(e => e.NBirthMonth).HasColumnName("nBirthMonth");

        entity.Property(e => e.NBirthYear).HasColumnName("nBirthYear");

        entity.Property(e => e.NDeathDay).HasColumnName("nDeathDay");

        entity.Property(e => e.NDeathMonth).HasColumnName("nDeathMonth");

        entity.Property(e => e.NDeathYear).HasColumnName("nDeathYear");

        entity.Property(e => e.NHasParents).HasColumnName("nHasParents");

        entity.Property(e => e.NHasSpouse).HasColumnName("nHasSpouse");

        entity.Property(e => e.NIsLiving).HasColumnName("nIsLiving");

        entity.Property(e => e.StrAdress)
                  .HasMaxLength(255)
                  .HasColumnName("strAdress");

        entity.Property(e => e.StrBurPlace)
                  .HasMaxLength(250)
                  .HasColumnName("strBurPlace");

        entity.Property(e => e.StrEheName)
                  .HasMaxLength(50)
                  .HasColumnName("strEheName");

        entity.Property(e => e.StrFatherId)
                  .HasMaxLength(8)
                  .HasColumnName("strFatherID");

        entity.Property(e => e.StrFullname)
                  .HasMaxLength(50)
                  .HasColumnName("strFullname");

        entity.Property(e => e.StrMarriedName)
                  .HasMaxLength(50)
                  .HasColumnName("strMarriedName");

        entity.Property(e => e.StrMotherId)
                  .HasMaxLength(8)
                  .HasColumnName("strMotherID");

        entity.Property(e => e.StrName)
                  .HasMaxLength(50)
                  .HasColumnName("strName");

        entity.Property(e => e.StrNick)
                  .HasMaxLength(50)
                  .HasColumnName("strNick");

        entity.Property(e => e.StrPersonId)
                  .IsRequired()
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");

        entity.Property(e => e.StrPreName)
                  .HasMaxLength(50)
                  .HasColumnName("strPreName");

        entity.Property(e => e.StrRace)
                  .HasMaxLength(50)
                  .HasColumnName("strRace");

        entity.Property(e => e.StrSex)
                  .HasMaxLength(1)
                  .HasColumnName("strSex")
                  .IsFixedLength(true);

        entity.Property(e => e.StrWork)
                  .HasMaxLength(250)
                  .HasColumnName("strWork");

        entity.Property(e => e.TikBirth)
                  .HasMaxLength(64)
                  .IsUnicode(false)
                  .HasColumnName("tikBirth");

        entity.Property(e => e.TikBurDate)
                  .HasMaxLength(64)
                  .IsUnicode(false)
                  .HasColumnName("tikBurDate");

        entity.Property(e => e.TkDeath)
                  .HasMaxLength(64)
                  .IsUnicode(false)
                  .HasColumnName("tkDeath");
      });

      modelBuilder.Entity<TPersonPortrait>(entity =>
      {
        entity.HasKey(e => e.NPersonPortraitId);

        entity.ToTable("tPersonPortrait");

        entity.Property(e => e.NPersonPortraitId).HasColumnName("nPersonPortraitID");

        entity.Property(e => e.DtCreate)
                  .HasColumnType("datetime")
                  .HasColumnName("dtCreate");

        entity.Property(e => e.DtUpdate)
                  .HasColumnType("datetime")
                  .HasColumnName("dtUpdate");

        entity.Property(e => e.NActive).HasColumnName("nActive");

        entity.Property(e => e.StrPdfName)
                  .IsRequired()
                  .HasMaxLength(255)
                  .HasColumnName("strPdfName");

        entity.Property(e => e.StrPersonId)
                  .IsRequired()
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");

        entity.Property(e => e.StrRemarks).HasColumnName("strRemarks");

        entity.Property(e => e.StrTitle)
                  .IsRequired()
                  .HasMaxLength(255)
                  .HasColumnName("strTitle");
      });

      modelBuilder.Entity<TRemark>(entity =>
      {
        entity.HasKey(e => e.NRemarkId);

        entity.ToTable("tRemarks");

        entity.Property(e => e.NRemarkId).HasColumnName("nRemarkID");

        entity.Property(e => e.BActiv).HasColumnName("bActiv");

        entity.Property(e => e.StrPersonId)
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");

        entity.Property(e => e.StrRemarks).HasColumnName("strRemarks");

        entity.Property(e => e.StrRemarksClean).HasColumnName("strRemarksClean");
      });

      modelBuilder.Entity<TUser>(entity =>
      {
        entity.HasKey(e => e.NUserId)
                  .HasName("PK_tUser");

        entity.ToTable("tUsers");

        entity.Property(e => e.NUserId).HasColumnName("nUserID");

        entity.Property(e => e.BActive).HasColumnName("bActive");

        entity.Property(e => e.BHasPaid).HasColumnName("bHasPaid");

        entity.Property(e => e.BMustNotPaid).HasColumnName("bMustNotPaid");

        entity.Property(e => e.DtAdmissionDate)
                  .HasColumnType("datetime")
                  .HasColumnName("dtAdmissionDate");

        entity.Property(e => e.DtCheckOutDate)
                  .HasColumnType("datetime")
                  .HasColumnName("dtCheckOutDate");

        entity.Property(e => e.DtPaid)
                  .HasColumnType("datetime")
                  .HasColumnName("dtPaid");

        entity.Property(e => e.NRole).HasColumnName("nRole");

        entity.Property(e => e.NZip).HasColumnName("nZip");

        entity.Property(e => e.StrAdress)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasColumnName("strAdress");

        entity.Property(e => e.StrCountry)
                  .HasMaxLength(50)
                  .HasColumnName("strCountry");

        entity.Property(e => e.StrEmail)
                  .HasMaxLength(50)
                  .HasColumnName("strEmail");

        entity.Property(e => e.StrLetter)
                  .HasMaxLength(50)
                  .HasColumnName("strLetter");

        entity.Property(e => e.StrLoginName)
                  .HasMaxLength(50)
                  .HasColumnName("strLoginName");

        entity.Property(e => e.StrName)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasColumnName("strName");

        entity.Property(e => e.StrPassword)
                  .HasMaxLength(50)
                  .HasColumnName("strPassword");

        entity.Property(e => e.StrPersonAccessList)
                  .HasMaxLength(255)
                  .HasColumnName("strPersonAccessList");

        entity.Property(e => e.StrPersonId)
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");

        entity.Property(e => e.StrPreName)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasColumnName("strPreName");

        entity.Property(e => e.StrRemarks)
                  .HasMaxLength(3000)
                  .HasColumnName("strRemarks");

        entity.Property(e => e.StrSalutation)
                  .HasMaxLength(20)
                  .HasColumnName("strSalutation");

        entity.Property(e => e.StrTel)
                  .HasMaxLength(50)
                  .HasColumnName("strTel");

        entity.Property(e => e.StrTown)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasColumnName("strTown");
      });

      modelBuilder.Entity<Tlemma>(entity =>
      {
        entity.ToTable("tlemma");

        entity.Property(e => e.Id)
                  .ValueGeneratedNever()
                  .HasColumnName("id");

        entity.Property(e => e.Created)
                  .HasColumnType("datetime")
                  .HasColumnName("created");

        entity.Property(e => e.Description).HasColumnName("description");

        entity.Property(e => e.Place).HasColumnName("place");

        entity.Property(e => e.Source).HasColumnName("source");

        entity.Property(e => e.Title)
                  .HasMaxLength(40)
                  .IsUnicode(false)
                  .HasColumnName("title");

        entity.Property(e => e.Voice)
                  .HasMaxLength(250)
                  .HasColumnName("voice");

        entity.Property(e => e.Word)
                  .HasMaxLength(100)
                  .IsUnicode(false)
                  .HasColumnName("word")
                  .IsFixedLength(true);

        entity.Property(e => e.StrPersonId)
                 .HasMaxLength(8)
                 .HasColumnName("strPersonID");
      });

      modelBuilder.Entity<VParentsChildren>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("vParentsChildrens");

        entity.Property(e => e.StrChildrenFullName)
                  .HasMaxLength(50)
                  .HasColumnName("strChildrenFullName");

        entity.Property(e => e.StrChildrenId)
                  .HasMaxLength(8)
                  .HasColumnName("strChildrenID");

        entity.Property(e => e.StrFatherFullName)
                  .HasMaxLength(50)
                  .HasColumnName("strFatherFullName");

        entity.Property(e => e.StrFatherId)
                  .HasMaxLength(8)
                  .HasColumnName("strFatherID");

        entity.Property(e => e.StrMotherFullName)
                  .HasMaxLength(50)
                  .HasColumnName("strMotherFullName");

        entity.Property(e => e.StrMotherId)
                  .HasMaxLength(8)
                  .HasColumnName("strMotherID");

        entity.Property(e => e.TikChildrenBirth)
                  .HasMaxLength(64)
                  .IsUnicode(false)
                  .HasColumnName("tikChildrenBirth");

        entity.Property(e => e.TikChildrenDeath)
                  .HasMaxLength(64)
                  .IsUnicode(false)
                  .HasColumnName("tikChildrenDeath");

        entity.Property(e => e.TikDivorceDate)
                  .HasMaxLength(50)
                  .HasColumnName("tikDivorceDate");

        entity.Property(e => e.TikMarriageDate)
                  .HasMaxLength(50)
                  .HasColumnName("tikMarriageDate");
      });

      modelBuilder.Entity<VPartner>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("vPartners");

        entity.Property(e => e.NBirthYear).HasColumnName("nBirthYear");

        entity.Property(e => e.NCurrent).HasColumnName("nCurrent");

        entity.Property(e => e.NDeathYear).HasColumnName("nDeathYear");

        entity.Property(e => e.NHasParents).HasColumnName("nHasParents");

        entity.Property(e => e.NHasSpouse).HasColumnName("nHasSpouse");

        entity.Property(e => e.NIsLiving).HasColumnName("nIsLiving");

        entity.Property(e => e.StrAdress)
                  .HasMaxLength(255)
                  .HasColumnName("strAdress");

        entity.Property(e => e.StrFatherId)
                  .HasMaxLength(8)
                  .HasColumnName("strFatherID");

        entity.Property(e => e.StrFullname)
                  .HasMaxLength(50)
                  .HasColumnName("strFullname");

        entity.Property(e => e.StrMarriedName)
                  .HasMaxLength(50)
                  .HasColumnName("strMarriedName");

        entity.Property(e => e.StrMotherId)
                  .HasMaxLength(8)
                  .HasColumnName("strMotherID");

        entity.Property(e => e.StrName)
                  .HasMaxLength(50)
                  .HasColumnName("strName");

        entity.Property(e => e.StrNick)
                  .HasMaxLength(50)
                  .HasColumnName("strNick");

        entity.Property(e => e.StrPartnerId)
                  .HasMaxLength(8)
                  .HasColumnName("strPartnerID");

        entity.Property(e => e.StrPersonId)
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");

        entity.Property(e => e.StrPreName)
                  .HasMaxLength(50)
                  .HasColumnName("strPreName");

        entity.Property(e => e.StrRace)
                  .HasMaxLength(50)
                  .HasColumnName("strRace");

        entity.Property(e => e.StrSex)
                  .HasMaxLength(1)
                  .HasColumnName("strSex")
                  .IsFixedLength(true);

        entity.Property(e => e.StrWork)
                  .HasMaxLength(250)
                  .HasColumnName("strWork");

        entity.Property(e => e.TikBirth)
                  .HasMaxLength(64)
                  .IsUnicode(false)
                  .HasColumnName("tikBirth");

        entity.Property(e => e.TikDivorceDate)
                  .HasMaxLength(50)
                  .HasColumnName("tikDivorceDate");

        entity.Property(e => e.TikMarriageDate)
                  .HasMaxLength(50)
                  .HasColumnName("tikMarriageDate");

        entity.Property(e => e.TkDeath)
                  .HasMaxLength(64)
                  .IsUnicode(false)
                  .HasColumnName("tkDeath");
      });

      modelBuilder.Entity<VPersonAdress>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("vPersonAdresses");

        entity.Property(e => e.StrAdress)
                  .HasMaxLength(255)
                  .HasColumnName("strAdress");

        entity.Property(e => e.StrPersonId)
                  .IsRequired()
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");
      });

      modelBuilder.Entity<VPersonRemark>(entity =>
      {
        entity.HasNoKey();

        entity.ToView("vPersonRemarks");

        entity.Property(e => e.BActiv).HasColumnName("bActiv");

        entity.Property(e => e.NActive).HasColumnName("nActive");

        entity.Property(e => e.NBirthDay).HasColumnName("nBirthDay");

        entity.Property(e => e.NBirthMonth).HasColumnName("nBirthMonth");

        entity.Property(e => e.NBirthYear).HasColumnName("nBirthYear");

        entity.Property(e => e.NDeathDay).HasColumnName("nDeathDay");

        entity.Property(e => e.NDeathMonth).HasColumnName("nDeathMonth");

        entity.Property(e => e.NDeathYear).HasColumnName("nDeathYear");

        entity.Property(e => e.NHasParents).HasColumnName("nHasParents");

        entity.Property(e => e.NHasSpouse).HasColumnName("nHasSpouse");

        entity.Property(e => e.NIsLiving).HasColumnName("nIsLiving");

        entity.Property(e => e.NPersonId).HasColumnName("nPersonID");

        entity.Property(e => e.NRemarkId).HasColumnName("nRemarkID");

        entity.Property(e => e.StrAdress)
                  .HasMaxLength(255)
                  .HasColumnName("strAdress");

        entity.Property(e => e.StrEheName)
                  .HasMaxLength(50)
                  .HasColumnName("strEheName");

        entity.Property(e => e.StrFatherId)
                  .HasMaxLength(8)
                  .HasColumnName("strFatherID");

        entity.Property(e => e.StrFullname)
                  .HasMaxLength(50)
                  .HasColumnName("strFullname");

        entity.Property(e => e.StrMarriedName)
                  .HasMaxLength(50)
                  .HasColumnName("strMarriedName");

        entity.Property(e => e.StrMotherId)
                  .HasMaxLength(8)
                  .HasColumnName("strMotherID");

        entity.Property(e => e.StrName)
                  .HasMaxLength(50)
                  .HasColumnName("strName");

        entity.Property(e => e.StrNick)
                  .HasMaxLength(50)
                  .HasColumnName("strNick");

        entity.Property(e => e.StrPersonId)
                  .IsRequired()
                  .HasMaxLength(8)
                  .HasColumnName("strPersonID");

        entity.Property(e => e.StrPreName)
                  .HasMaxLength(50)
                  .HasColumnName("strPreName");

        entity.Property(e => e.StrRace)
                  .HasMaxLength(50)
                  .HasColumnName("strRace");

        entity.Property(e => e.StrRemarks).HasColumnName("strRemarks");

        entity.Property(e => e.StrSex)
                  .HasMaxLength(1)
                  .HasColumnName("strSex")
                  .IsFixedLength(true);

        entity.Property(e => e.StrWork)
                  .HasMaxLength(250)
                  .HasColumnName("strWork");

        entity.Property(e => e.TikBirth)
                  .HasMaxLength(64)
                  .IsUnicode(false)
                  .HasColumnName("tikBirth");

        entity.Property(e => e.TkDeath)
                  .HasMaxLength(64)
                  .IsUnicode(false)
                  .HasColumnName("tkDeath");
      });

      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}
