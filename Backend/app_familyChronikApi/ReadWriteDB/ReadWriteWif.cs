using app_familyBackend.DataContext;
using appAhnenforschungData.Models.App;
using appAhnenforschungData.Models.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace app_familyChronikApi.ReadWriteDB;
{
  public class ReadWriteWif(MyDatabaseContext context)
  {
    private readonly MyDatabaseContext _context = context;


    #region Personenbilder

    public List<ValueObject.ImagePerson> GetPublicImagePersons()
    {
      try
      {
        List<CImagePerson> arl = new List<CImagePerson>();

        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1).OrderByDescending(x => x.AddDate).OrderByDescending(x => x.InProgress))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          //ppImagePersonEntityToModel(ref pti, t);
          arl.Add(pti);
        }

        return arl;
      }
      catch (Exception)
      {
        return new List<CImagePerson>();
      }
    }

    /// <summary>
    /// Alle Fotos die der normale Benutzer sechen soll
    /// </summary>
    /// <returns></returns>
    public List<CImagePerson> GetPublicImagePersonsOverviewLimited()
    {
      try
      {
        List<CImagePerson> arl = new List<CImagePerson>();

        // in Progress
        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.InProgress == true && x.Active == true).OrderByDescending(x => x.AddDate))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          if (arl.Find(x => x.Id == pti.Id) == null)
          {
            arl.Add(pti);
          }
        }
        return arl.OrderByDescending(x => x.InProgress).OrderBy(x => x.IsArchivated).ToList();
      }
      catch (Exception)
      {
        return new List<CImagePerson>();
      }
    }


    /// <summary>
    /// Alle Fotos die der normale Benutzer sechen soll
    /// </summary>
    /// <returns></returns>
    public List<CImagePerson> GetPublicImagePersonsOverview()
    {
      try
      {
        List<CImagePerson> arl = new List<CImagePerson>();

        // in Progress
        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.InProgress == true && x.Active == true).OrderByDescending(x => x.AddDate))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          if (arl.Find(x => x.Id == pti.Id) == null)
          {
            arl.Add(pti);
          }
        }


        // Archiviert
        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.IsArchivated == true && x.IsExported == false && x.Active == true).OrderByDescending(x => x.AddDate))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          if (arl.Find(x => x.Id == pti.Id) == null)
          {
            arl.Add(pti);
          }
        }

        // Exportiert
        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.IsExported == true && x.Active == true).OrderByDescending(x => x.AddDate))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          if (arl.Find(x => x.Id == pti.Id) == null)
          {
            arl.Add(pti);
          }
        }

        // Akive
        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.InProgress == false && x.IsArchivated == false && x.IsExported == false && x.Active == true).OrderByDescending(x => x.AddDate))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          if (arl.Find(x => x.Id == pti.Id) == null)
          {
            arl.Add(pti);
          }
        }
        return arl.OrderByDescending(x => x.InProgress).OrderBy(x => x.IsArchivated).ToList();
      }
      catch (Exception)
      {
        return new List<CImagePerson>();
      }
    }

    /// <summary>
    /// all images listed and sorted.
    /// </summary>
    /// <returns></returns>
    public List<CImagePerson> GetAllImagePersons()
    {
      try
      {
        List<CImagePerson> arl = new List<CImagePerson>();

        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1).OrderByDescending(x => x.AddDate).OrderByDescending(x => x.InProgress))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          arl.Add(pti);
        }

        return arl.OrderByDescending(x => x.InProgress).OrderBy(x => x.IsArchivated).ToList();
      }
      catch (Exception)
      {
        return new List<CImagePerson>();
      }
    }

    /// <summary>
    /// all active images listed and sorted.
    /// </summary>
    /// <returns></returns>
    public List<CImagePerson> GetActiveImagePersons()
    {
      try
      {
        List<CImagePerson> arl = new List<CImagePerson>();

        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.Active == true).OrderByDescending(x => x.AddDate).OrderByDescending(x => x.InProgress))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          arl.Add(pti);
        }

        return arl.OrderByDescending(x => x.InProgress).OrderBy(x => x.IsArchivated).ToList();
      }
      catch (Exception)
      {
        return new List<CImagePerson>();
      }
    }

    private List<CImagePersonPosition> GetImagePersonsPositionByImageId(int idImage)
    {
      try
      {
        List<CImagePersonPosition> arl = new List<CImagePersonPosition>();

        foreach (TImagePersonsPosition t in db.TImagePersonsPositions.Where(x => x.IdImagePerson == idImage))
        {
          CImagePersonPosition pos = new CImagePersonPosition();
          MappImagePersonPositionEntityToModel(ref pos, t);
          arl.Add(pos);
        }
        return arl;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    /// <summary>
    /// Insert on position
    /// </summary>
    /// <param name="IdImage"></param>
    /// <returns>New CImagePersonPosition</returns>
    public CImagePersonPosition InsertImagePosition(int IdImage)
    {
      try
      {
        using (IDbContextTransaction transaction = db.Database.BeginTransaction())
        {
          try
          {

            CImagePersonPosition imagePosition = null;
            CImagePerson image = null;
            int pos = 1;
            int newID = -1;

            if (db.TImagePersonsPositions.Any(x => x.IdImagePerson == IdImage))
            {
              pos = db.TImagePersonsPositions.Where(x => x.IdImagePerson == IdImage).Max(x => x.Pos);
              pos = pos + 1;
            }

            TImagePersonsPosition t1 = CreateEmptyImagePersonPositionEntity(IdImage);
            t1.Pos = pos;
            t1.Active = true;
            db.TImagePersonsPositions.Add(t1);
            db.SaveChanges();
            newID = t1.Id;
            imagePosition = GetImagePersonsPositionById(newID);
            image = GetImagePersonsById(imagePosition.IdImagePerson, withPositions: true);
            image.PositionsCount = image.PositionsCount + 1;
            UpdatePositionCount(image, transaction);

            transaction.Commit();

            return imagePosition;
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            throw ex;
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Leerstruktur
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    private CImagePersonPosition CreateEmptyImagePersonPositionModel(int imageId)
    {
      CImagePersonPosition oPosition = new CImagePersonPosition();
      oPosition.IdPersonPosition = -1;
      oPosition.IdImagePerson = imageId;
      oPosition.Pos = 0;
      oPosition.PersonName = string.Empty;
      oPosition.PersonPreName = string.Empty;
      oPosition.PersonAddress = string.Empty;
      oPosition.PersonHouseNo = string.Empty;
      oPosition.PersonZip = string.Empty;
      oPosition.PersonCountry = string.Empty;
      oPosition.PersonTown = string.Empty;
      oPosition.PersonBirthYear = 0;
      oPosition.ReferencePersonId = string.Empty;
      oPosition.PersonDescription = string.Empty;
      oPosition.EditContactData = string.Empty;
      oPosition.EditEmail = string.Empty;
      oPosition.Person_Add_Date = DateTime.Now;
      oPosition.Person_Upd_Date = DateTime.Now;
      oPosition.PersonFinish = false;
      oPosition.PersonActive = false;

      return oPosition;
    }

    /// <summary>
    /// Leerstruktur
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    private TImagePersonsPosition CreateEmptyImagePersonPositionEntity(int imageId)
    {
      TImagePersonsPosition tEmpty = new TImagePersonsPosition();
      tEmpty.IdImagePerson = imageId;
      tEmpty.Pos = 0;
      tEmpty.Name = string.Empty;
      tEmpty.PreName = string.Empty;
      tEmpty.Address = string.Empty;
      tEmpty.HouseNo = string.Empty;
      tEmpty.Zip = string.Empty;
      tEmpty.Country = string.Empty;
      tEmpty.Town = string.Empty;
      tEmpty.BirthYear = 0;
      tEmpty.ReferencePersonId = string.Empty;
      tEmpty.Description = string.Empty;
      tEmpty.EditContactData = string.Empty;
      tEmpty.EditEmail = string.Empty;
      tEmpty.AddDate = DateTime.Now;
      tEmpty.UpdDate = DateTime.Now;
      tEmpty.Finish = false;
      tEmpty.Active = false;

      return tEmpty;
    }

    public CImagePerson CreateEmptyImagePersons(CImagePerson oImage)
    {
      oImage.Id = -1;
      oImage.FileName = "";
      oImage.OriginalFileName = "?";
      oImage.ImagePath = "http://placehold.it/750x550";
      oImage.Title = "";
      oImage.Description = "";
      oImage.PositionsCount = 0;
      oImage.Add_Date = DateTime.Now;
      oImage.Active = false;
      oImage.InProgress = false;
      oImage.IsArchivated = false;

      oImage.IsExported = false;
      oImage.SourceDescription = "";
      oImage.SourceImageFileName = "";

      oImage.ImagePersonsPositions = new List<CImagePersonPosition>();
      CImagePersonPosition oPosition;
      for (int i = 0; i < oImage.PositionsCount; i++)
      {
        oPosition = CreateEmptyImagePersonPositionModel(oImage.Id);
        oPosition.Pos = i + 1;
        oPosition.PersonActive = true;

        oImage.ImagePersonsPositions.Add(oPosition);
      }
      return oImage;
    }

    private CImagePersonPosition CreateEmptyCreateOrUpdateImagePersonsPosition(int idImage, int maxPosition, bool withUpdate, IDbContextTransaction transaction)
    {
      try
      {
        TImagePersonsPosition tPosAdd = CreateEmptyImagePersonPositionEntity(idImage);
        CImagePersonPosition pos = new CImagePersonPosition();
        tPosAdd.Pos = maxPosition;
        tPosAdd.Active = true;
        MappImagePersonPositionEntityToModel(ref pos, tPosAdd);
        if (withUpdate)
        {
          db.TImagePersonsPositions.Attach(tPosAdd);
          db.Entry(tPosAdd).State = EntityState.Added;
          db.SaveChanges();
        }

        return pos;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Die Positinen für ein Bild erstellen
    /// </summary>
    /// <param name="idImage"></param>
    public void InsertPositionsToImagePostion(int idImage)
    {
      using (IDbContextTransaction transaction = db.Database.BeginTransaction())
      {
        try
        {
          TImagePerson tImg = db.TImagePersons.FirstOrDefault(x => x.Id == idImage);
          int countPos = db.TImagePersonsPositions.Where(x => x.IdImagePerson == idImage).Count();
          if (tImg.PositionsCount > countPos)
          {
            if (tImg != null)
            {
              if (countPos == 0)
              {
                for (int i = 1; i <= tImg.PositionsCount; i++)
                {
                  CreateEmptyCreateOrUpdateImagePersonsPosition(tImg.Id, i, withUpdate: true, transaction: transaction);
                }
              }
              else
              {
                // nur wenn noch keine Position bearbeitet wurde.
                if (!db.TImagePersonsPositions.Any(x => x.Finish == true))
                {
                  int maxPos = db.TImagePersonsPositions.Where(x => x.IdImagePerson == tImg.Id).Max(x => x.Pos);
                  if (maxPos <= countPos)
                  {
                    maxPos = maxPos + 1; ;
                    for (int i = maxPos; i <= tImg.PositionsCount; i++)
                    {
                      CreateEmptyCreateOrUpdateImagePersonsPosition(tImg.Id, i, withUpdate: true, transaction: transaction);
                    }
                  }
                }
              }
            }
          }
          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          throw ex;
        }
      }
    }

    /// <summary>
    /// Bearbeitungsfortschritt
    /// </summary>
    /// <param name="onlyActive"></param>
    /// <returns></returns>
    public List<CImagePersonProgress> CalcWorkProgress(bool onlyActive)
    {
      try
      {
        List<CImagePersonProgress> arlProgress = new List<CImagePersonProgress>();
        List<CImagePerson> oImages = null;
        CImagePersonProgress oProgress = null;

        if (onlyActive)
        {
          oImages = GetActiveImagePersons();
        }
        else
        {
          oImages = GetAllImagePersons();
        }

        foreach (CImagePerson image in oImages)
        {
          oProgress = new CImagePersonProgress();
          Int32 count = GetImagePersonsPositionByImageId(image.Id).Count(x => x.PersonName != string.Empty && x.PersonPreName != string.Empty);
          oProgress.Id = image.Id;
          oProgress.QuantityReady = count;
          oProgress.Quantity = image.PositionsCount;
          oProgress.Percent = (100.00 / oProgress.Quantity) * oProgress.QuantityReady;
          if (oProgress.Percent > 100)
          {
            oProgress.Percent = 100;
          }
          oProgress.PercentDisplay = Math.Ceiling(oProgress.Percent) + "%";
          oProgress.ProgressClass = "progress-bar progress-bar bg-warning";
          Int32 countFinish = GetImagePersonsPositionByImageId(image.Id).Count(x => x.ReferencePersonId != string.Empty);
          if (oProgress.Quantity > 0 && oProgress.Quantity == oProgress.QuantityReady && oProgress.QuantityReady == countFinish)
          {
            oProgress.ProgressClass = "progress-bar progress-bar-success";
          }

          arlProgress.Add(oProgress);
        }

        return arlProgress;
      }
      catch (Exception ex)
      {
        throw ex;
      }

    }

    /// <summary>
    /// exist changes
    /// </summary>
    /// <param name="oClientImage"></param>
    /// <returns></returns>
    public bool IsModifiedImagePersonsOrPositions(CImagePerson oClientImage)
    {
      try
      {
        bool modified = false;
        CImagePerson oServerImage = GetImagePersonsById(oClientImage.Id, withPositions: true);

        CImagePersonPosition posClient;
        foreach (CImagePersonPosition posServer in oClientImage.ImagePersonsPositions)
        {
          posClient = oServerImage.ImagePersonsPositions.FirstOrDefault(x => x.IdPersonPosition == posServer.IdPersonPosition);
          if (posClient != null)
          {
            if (!posClient.Equals(posServer))
            {
              modified = true;
              break;
            }
          }
        }

        if (!modified)
        {
          if (oServerImage != null)
          {
            if (!oClientImage.Equals(oServerImage))
            {
              modified = true;
            }
          }
        }
        return modified;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    /// <summary>
    ///  Get imge with or non positions
    /// </summary>
    /// <param name="idImage"></param>
    /// <param name="withPositions"></param>
    /// <returns></returns>
    public CImagePerson GetImagePersonsById(int idImage, bool withPositions)
    {
      try
      {
        CImagePerson oImage = new CImagePerson();
        if (idImage > 0)
        {
          TImagePerson tImage = db.TImagePersons.FirstOrDefault(x => x.Id == idImage);

          oImage.ImagePersonsPositions = new List<CImagePersonPosition>();
          MappImagePersonEntityToModel(ref oImage, tImage);
          if (withPositions)
          {
            oImage.ImagePersonsPositions = GetImagePersonsPositionByImageId(oImage.Id);
          }
        }
        return oImage;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// All images for in person
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public List<CImagePerson> GetImagePositionWithPositionsByPerson(string person)
    {
      try
      {
        List<CImagePerson> oImages = new List<CImagePerson>();
        var persons = new List<int>();
        {
          foreach (TImagePersonsPosition pos in db.TImagePersonsPositions.Where(x => x.ReferencePersonId == person && x.Active == true && x.Finish == true))
          {
            if (!persons.Contains(pos.IdImagePerson))
            {
              persons.Add(pos.IdImagePerson);
            }

          }

          foreach (TImagePerson image in db.TImagePersons.Where(x => x.Active == true && persons.Contains(x.Id)))
          {
            var img = GetImagePersonsById(image.Id, withPositions: true);
            oImages.Add(img);
          }
          return oImages;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// On image wirth all pos
    /// </summary>
    /// <param name="imagePersonId"></param>
    /// <returns></returns>
    public CImagePerson GetImagePersonWithPositionsById(int imagePersonId)
    {
      try
      {
        CImagePerson oImages = new CImagePerson();
        oImages = GetImagePersonsById(imagePersonId, withPositions: true);
        return oImages;

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }


    public CImagePersonPosition GetImagePersonsPositionById(int idPosition)
    {
      try
      {
        TImagePersonsPosition tPos = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == idPosition);
        CImagePersonPosition oPos = new CImagePersonPosition();

        if (tPos != null)
        {
          MappImagePersonPositionEntityToModel(ref oPos, tPos);
        }
        return oPos;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Default Adresse
    /// </summary>
    /// <param name="imagePersonPosition"></param>
    /// <returns></returns>
    public CImagePersonPosition UseImagePersonPositionDefaultValues(CImagePersonPosition imagePersonPosition)
    {
      try
      {
        string country = "Liechtenstein";
        if (imagePersonPosition.PersonZip != null && imagePersonPosition.PersonZip.Trim().Length == 4)
        {
          switch (imagePersonPosition.PersonZip)
          {
            case "9485":
              imagePersonPosition.PersonTown = "Nendeln";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9486":
              imagePersonPosition.PersonTown = "Schaanwald";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9487":
              imagePersonPosition.PersonTown = "Bendern";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9488":
              imagePersonPosition.PersonTown = "Schellenberg";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9489":
              imagePersonPosition.PersonTown = "Vaduz";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9490":
              imagePersonPosition.PersonTown = "Vaduz";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9491":
              imagePersonPosition.PersonTown = "Ruggell";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9492":
              imagePersonPosition.PersonTown = "Eschen";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9493":
              imagePersonPosition.PersonTown = "Mauren";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9494":
              imagePersonPosition.PersonTown = "Schaan";
              imagePersonPosition.PersonCountry = "Liechtenstein";
              break;
            case "9495":
              imagePersonPosition.PersonTown = "Triesen";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9496":
              imagePersonPosition.PersonTown = "Balzers";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9497":
              imagePersonPosition.PersonTown = "Triesenberg";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9498":
              imagePersonPosition.PersonTown = "Planken";
              imagePersonPosition.PersonCountry = country;
              break;
          }
        }
        else if (imagePersonPosition.PersonTown != null)
        {
          switch (imagePersonPosition.PersonTown)
          {
            case "Nendeln":
              imagePersonPosition.PersonZip = "9485";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Schaanwald":
              imagePersonPosition.PersonZip = "9486";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Bendern":
              imagePersonPosition.PersonZip = "9487";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Schellenberg":
              imagePersonPosition.PersonZip = "9488";
              imagePersonPosition.PersonCountry = country;
              break;
            //case "Vaduz":
            //  imagePersonPosition.PersonZip = "9489";
            //  imagePersonPosition.PersonCountry = country;
            //  break;
            case "Vaduz":
              imagePersonPosition.PersonZip = "9490";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Ruggell":
              imagePersonPosition.PersonZip = "9491";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Eschen":
              imagePersonPosition.PersonZip = "9492";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Mauren":
              imagePersonPosition.PersonZip = "9493";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Schaan":
              imagePersonPosition.PersonZip = "9494";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Triesen":
              imagePersonPosition.PersonZip = "9495";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Balzers":
              imagePersonPosition.PersonZip = "9496";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Triesenberg":
              imagePersonPosition.PersonZip = "9497";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Planken":
              imagePersonPosition.PersonZip = "9498";
              imagePersonPosition.PersonCountry = country;
              break;
          }
        }
        return imagePersonPosition;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public CImagePerson CreateOrUpdateImagePersons(CImagePerson oImage)
    {
      try
      {
        TImagePerson t = db.TImagePersons.FirstOrDefault(x => x.Id == oImage.Id);
        CImagePerson oResultImage;

        if (t == null)
        {
          int newID = -1;
          t = new TImagePerson();
          MappImagePersonModelToEntity(oImage, ref t);
          //using (Context.AhnenforschungEntityDataModel entities = new Context.AhnenforschungEntityDataModel())
          //{
          using (IDbContextTransaction transaction = db.Database.BeginTransaction())
          {
            try
            {
              db.TImagePersons.Add(t);
              db.Entry(t).State = EntityState.Added;
              db.SaveChanges();
              newID = t.Id;

              CImagePerson oImageAdded = new CImagePerson
              {
                ImagePersonsPositions = new List<CImagePersonPosition>()
              };
              t = db.TImagePersons.FirstOrDefault(x => x.Id == newID);
              MappImagePersonEntityToModel(ref oImageAdded, t);
              oImageAdded.ImagePersonsPositions = oImage.ImagePersonsPositions;
              CreateOrUpdateImagePersonPositions(oImageAdded.ImagePersonsPositions, transaction);
              oResultImage = oImageAdded;

              transaction.Commit();
            }
            catch (Exception ex)
            {
              transaction.Rollback();
              throw ex;
            }

          }
          // }
        }
        else
        {
          using (IDbContextTransaction transaction = db.Database.BeginTransaction())
          {
            try
            {
              MappImagePersonModelToEntity(oImage, ref t);
              db.TImagePersons.Attach(t);
              db.Entry(t).State = EntityState.Modified;
              db.SaveChanges();
              CreateOrUpdateImagePersonPositions(oImage.ImagePersonsPositions, transaction);
              oResultImage = GetImagePersonsById(oImage.Id, withPositions: true);
              transaction.Commit();
            }
            catch (Exception ex)
            {
              transaction.Rollback();
              throw ex;
            }
          }

        }
        return oResultImage;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public void UpdateImagePersonsDescription(CImagePerson oImage)
    {
      try
      {
        TImagePerson t = db.TImagePersons.FirstOrDefault(x => x.Id == oImage.Id);
        t.Description = oImage.Description;
        db.TImagePersons.Attach(t);
        db.Entry(t).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public void UpdateImagePersonPositionDescription(CImagePersonPosition oPos)
    {
      try
      {
        TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == oPos.IdPersonPosition);
        t.Description = oPos.PersonDescription;
        db.TImagePersonsPositions.Attach(t);
        db.Entry(t).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private void UpdatePositionCount(CImagePerson oImage, IDbContextTransaction transaction)
    {
      try
      {
        TImagePerson t = db.TImagePersons.FirstOrDefault(x => x.Id == oImage.Id);
        t.PositionsCount = oImage.PositionsCount;
        db.TImagePersons.Attach(t);
        db.Entry(t).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    public CImagePersonPosition SetImagePersonPositionToFinish(int id)
    {
      try
      {
        TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == id);
        t.Finish = !t.Finish;
        t.UpdDate = DateTime.Now;
        db.TImagePersonsPositions.Attach(t);
        db.Entry(t).State = EntityState.Modified;
        db.SaveChanges();
        return GetImagePersonsPositionById(id);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Status Bild ist exportiert 
    /// </summary>
    /// <param name="idImpage"></param>
    /// <param name="withPositions"></param>
    /// <returns>Changed Person image with or no positions</returns>
    public CImagePerson SetStateExported(int idImpage, bool withPositions)
    {
      try
      {
        TImagePerson t = db.TImagePersons.FirstOrDefault(x => x.Id == idImpage);
        t.IsExported = true;
        db.TImagePersons.Attach(t);
        db.Entry(t).State = EntityState.Modified;
        db.SaveChanges();
        return GetImagePersonsById(idImpage, withPositions);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Eine Position speichern
    /// </summary>
    /// <param name="modifiedPosition"></param>
    public CImagePersonPosition UpdateImagePersonPosition(CImagePersonPosition modifiedPosition)
    {
      try
      {
        using (IDbContextTransaction transaction = db.Database.BeginTransaction())
        {
          try
          {
            CReadWriteData oRwd = new CReadWriteData();
            if (modifiedPosition != null)
            {
              TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == modifiedPosition.IdPersonPosition);
              if (t != null)
              {
                t.UpdDate = DateTime.Now;
                modifiedPosition = UseImagePersonPositionDefaultValues(modifiedPosition);
                oRwd.MappImagePersonPositionModelToEntity(modifiedPosition, ref t);
                modifiedPosition.Person_Upd_Date = DateTime.Now;
                db.TImagePersonsPositions.Attach(t);
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                InsertImagePersonPositionHistory(null, modifiedPosition, transaction);
                transaction.Commit();
              }
            }
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            throw ex;
          }
        }

        return GetImagePersonsPositionById(modifiedPosition.IdPersonPosition);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    private void CreateOrUpdateImagePersonPositions(List<CImagePersonPosition> Positions, IDbContextTransaction transaction)
    {
      try
      {
        CReadWriteData oRwd = new CReadWriteData();
        if (Positions != null)
        {
          foreach (CImagePersonPosition ptiPos in Positions)
          {
            TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == ptiPos.IdPersonPosition);
            if (t != null)
            {
              t.UpdDate = DateTime.Now;
              oRwd.MappImagePersonPositionModelToEntity(ptiPos, ref t);
              db.TImagePersonsPositions.Attach(t);
              db.Entry(t).State = EntityState.Modified;
              db.SaveChanges();
              InsertImagePersonPositionHistory(null, ptiPos, transaction);
            }
            else
            {
              t = new TImagePersonsPosition();
              oRwd.MappImagePersonPositionModelToEntity(ptiPos, ref t);
              db.TImagePersonsPositions.Attach(t);
              db.Entry(t).State = EntityState.Added;
              db.SaveChanges();
              InsertImagePersonPositionHistory(null, ptiPos, transaction);
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    public void UpdateModifiedImagePersonPositions(CImagePerson ModifiedImagePersons, CImagePerson SessionImagePersons)
    {
      using (IDbContextTransaction transaction = db.Database.BeginTransaction())
      {
        try
        {
          TImagePersonsPosition tPos = null;
          CImagePersonPosition posSession;
          foreach (CImagePersonPosition posModified in ModifiedImagePersons.ImagePersonsPositions)
          {
            posSession = SessionImagePersons.ImagePersonsPositions.FirstOrDefault(x => x.IdPersonPosition == posModified.IdPersonPosition);
            if (posSession != null)
            {
              if (!posSession.Equals(posModified))
              {
                tPos = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == posModified.IdPersonPosition);
                if (tPos != null)
                {
                  MappImagePersonPositionModelToEntity(posModified, ref tPos);
                  tPos.UpdDate = DateTime.Now;
                  db.TImagePersonsPositions.Attach(tPos);
                  db.Entry(tPos).State = EntityState.Modified;
                  db.SaveChanges();
                }
              }
            }
          }

          if (!SessionImagePersons.Equals(ModifiedImagePersons))
          {
            TImagePerson tImage = db.TImagePersons.FirstOrDefault(x => x.Id == ModifiedImagePersons.Id);
            if (tImage != null)
            {
              MappImagePersonModelToEntity(ModifiedImagePersons, ref tImage);
              db.TImagePersons.Attach(tImage);
              db.Entry(tImage).State = EntityState.Modified;
              db.SaveChanges();
            }
          }
          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          throw ex;
        }
      }
    }

    /// <summary>
    ///  Eine Liste von Positionen speichern
    /// </summary>
    /// <param name="Positions"></param>
    public void CreateOrUpdateImagePersonPositions(List<CImagePersonPosition> positions)
    {
      using (IDbContextTransaction transaction = db.Database.BeginTransaction())
      {
        try
        {
          CReadWriteData oRwd = new CReadWriteData();
          if (positions != null)
          {
            foreach (CImagePersonPosition ptiPos in positions)
            {
              TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == ptiPos.IdPersonPosition);
              if (t != null)
              {
                t.UpdDate = DateTime.Now;
                oRwd.MappImagePersonPositionModelToEntity(ptiPos, ref t);
                db.TImagePersonsPositions.Attach(t);
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                InsertImagePersonPositionHistory(null, ptiPos, transaction);
              }
              else
              {
                t = new TImagePersonsPosition();
                oRwd.MappImagePersonPositionModelToEntity(ptiPos, ref t);
                db.TImagePersonsPositions.Attach(t);
                db.Entry(t).State = EntityState.Added;
                db.SaveChanges();
              }
            }
          }
          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          throw ex;
        }
      }
    }


    /// <summary>
    /// Loggen
    /// </summary>
    /// <param name="oImage"></param>
    /// <param name="oPosition"></param>
    /// <param name="transaction"></param>
    private void InsertImagePersonPositionHistory(CImagePerson oImage, CImagePersonPosition oPosition, IDbContextTransaction transaction)
    {
      try
      {
        TImagePersonsPositionsHistory t = new TImagePersonsPositionsHistory();
        t.IdImagePersonPosition = oPosition.IdPersonPosition;
        t.IdImagePerson = oPosition.IdImagePerson;
        t.Pos = oPosition.Pos;
        t.Name = CGlobal.ConvertNullToString(oPosition.PersonName);
        t.PreName = CGlobal.ConvertNullToString(oPosition.PersonPreName);
        t.Address = CGlobal.ConvertNullToString(oPosition.PersonAddress);
        t.HouseNo = CGlobal.ConvertNullToString(oPosition.PersonHouseNo);
        t.Zip = CGlobal.ConvertNullToString(oPosition.PersonZip);
        t.Country = CGlobal.ConvertNullToString(oPosition.PersonCountry);
        t.Town = CGlobal.ConvertNullToString(oPosition.PersonTown);
        t.BirthYear = oPosition.PersonBirthYear;
        t.ReferencePersonId = CGlobal.ConvertNullToString(oPosition.ReferencePersonId);
        if (oImage != null)
        {
          t.ImageDescription = CGlobal.ConvertNullToString(oImage.Description);
        }
        else
        {
          t.ImageDescription = string.Empty;
        }

        t.PersonDescription = CGlobal.ConvertNullToString(oPosition.PersonDescription);
        t.EditContactData = CGlobal.ConvertNullToString(oPosition.EditContactData);
        t.EditEmail = CGlobal.ConvertNullToString(oPosition.EditEmail);
        t.AddDate = oPosition.Person_Add_Date;
        t.UpdDate = oPosition.Person_Upd_Date;
        t.Finish = oPosition.PersonFinish;
        t.Active = oPosition.PersonActive;
        t.AddHistoryDate = DateTime.Now;

        db.TImagePersonsPositionsHistories.Attach(t);
        db.Entry(t).State = EntityState.Added;
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    /// <summary>
    /// Delete Image with all positions
    /// </summary>
    /// <param name="id"></param>
    public void DeleteImagePersonsByImageId(int id)
    {
      using (IDbContextTransaction transaction = db.Database.BeginTransaction())
      {
        try
        {
          TImagePerson t = db.TImagePersons.FirstOrDefault(x => x.Id == id);
          db.TImagePersons.Attach(t);
          db.Entry(t).State = EntityState.Deleted;
          db.SaveChanges();

          DeleteImagePersonPositionsByImageId(id, transaction);

          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          throw ex;
        }
      }
    }

    /// <summary>
    /// Delete all positions
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tx"></param>
    public void DeleteImagePersonPositionsByImageId(int id, IDbContextTransaction tx)
    {
      try
      {
        if (id > 0)
        {
          foreach (TImagePersonsPosition t in db.TImagePersonsPositions.Where(x => x.IdImagePerson == id))
          {
            db.TImagePersonsPositions.Attach(t);
            db.Entry(t).State = EntityState.Deleted;
            db.SaveChanges();
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Delete all positions
    /// </summary>
    /// <param name="id"></param>
    public void DeleteImagePersonPositionsByImageId(int id)
    {
      if (id > 0)
      {
        using (IDbContextTransaction transaction = db.Database.BeginTransaction())
        {
          try
          {
            foreach (TImagePersonsPosition t in db.TImagePersonsPositions.Where(x => x.IdImagePerson == id))
            {
              db.TImagePersonsPositions.Attach(t);
              db.Entry(t).State = EntityState.Deleted;
              db.SaveChanges();
            }
            transaction.Commit();
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            throw ex;
          }
        }
      }
    }

    /// <summary>
    /// Is CreateOrUpdateImagePersons by id active
    /// </summary>
    /// <param name="idImage"></param>
    /// <returns></returns>
    public bool IsImagePersonActive(int idImage)
    {
      try
      {
        bool isActive = false;

        if (idImage > 0)
        {
          TImagePerson tImage = db.TImagePersons.FirstOrDefault(x => x.Id == idImage);
          if (tImage != null)
          {
            isActive = tImage.Active.Value;
          }
        }

        return isActive;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    /// <summary>
    /// Is CreateOrUpdateImagePersons by position active
    /// </summary>
    /// <param name="idPosition"></param>
    /// <returns>true or false</returns>
    public bool IsImagePersonsActiveByPositionsById(int idPosition)
    {
      try
      {
        bool isActive = false;

        if (idPosition > 0)
        {
          TImagePersonsPosition tPos = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == idPosition);
          if (tPos != null)
          {
            TImagePerson tImage = db.TImagePersons.FirstOrDefault(x => x.Id == tPos.IdImagePerson);
            isActive = tImage.Active.Value;
          }
        }

        return isActive;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    /// <summary>
    /// Delete on single position.
    /// </summary>
    /// <param name="id"></param>
    public int DeleteImagePersonPosition(int id)
    {
      try
      {
        using (IDbContextTransaction transaction = db.Database.BeginTransaction())
        {
          try
          {
            int personImageId = -1;
            if (id > 0)
            {
              TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == id);
              if (!t.Finish)
              {
                personImageId = t.IdImagePerson;
                RecalcImagePersonPositions(t.Pos, transaction); // Positionen neu berechnen
                db.TImagePersonsPositions.Attach(t);
                db.Entry(t).State = EntityState.Deleted;
                db.SaveChanges();
                int countPos = db.TImagePersonsPositions.Where(x => x.IdImagePerson == personImageId).Count();
                var image = GetImagePersonsById(personImageId, withPositions: false);
                image.PositionsCount = countPos;
                UpdatePositionCount(image, transaction);

              }
            }

            transaction.Commit();

            return personImageId;
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            throw ex;
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Datensatz als gelöscht markieren.
    /// </summary>
    /// <param name="idPosition"></param>
    public void RemoveDeleteImagePersonPosition(CImagePersonPosition oPosition)
    {
      try
      {
        if (oPosition != null)
        {
          TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == oPosition.IdPersonPosition);
          if (!t.Finish)
          {
            //oPosition.PersonName = "gelöscht";
            //oPosition.PersonPreName = "gelöscht";
            //oPosition.PersonBirthYear = 0;
            //oPosition.PersonAddress = "gelöscht";
            //oPosition.PersonCountry = "gelöscht"; ;
            //oPosition.PersonDescription = "gelöscht";
            //oPosition.EditContactData = "gelöscht";
            //oPosition.EditEmail = "gelöscht";
            //oPosition.PersonHouseNo = "gelöscht";
            //oPosition.PersonTown = "gelöscht";
            //oPosition.PersonZip = "0000";

            oPosition.PersonName = null;
            oPosition.PersonPreName = null;
            oPosition.PersonBirthYear = 0;
            oPosition.PersonAddress = null;
            oPosition.PersonCountry = null;
            oPosition.PersonDescription = null;
            oPosition.EditContactData = null;
            oPosition.EditEmail = null;
            oPosition.PersonHouseNo = null;
            oPosition.PersonTown = null;
            oPosition.PersonZip = null;


            UpdateImagePersonPosition(oPosition);
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Recalc after deleted position
    /// </summary>
    /// <param name="StartPosition"></param>
    private void RecalcImagePersonPositions(int StartPosition, IDbContextTransaction transaction)
    {
      try
      {
        if (StartPosition > 0)
        {
          int i = StartPosition;
          foreach (TImagePersonsPosition t in db.TImagePersonsPositions.Where(x => x.Pos > StartPosition).OrderBy(x => x.Pos))
          {
            t.Pos = StartPosition;
            db.TImagePersonsPositions.Attach(t);
            db.Entry(t).State = EntityState.Deleted;
            db.SaveChanges();
            StartPosition += 1;
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
    #endregion

  }
}