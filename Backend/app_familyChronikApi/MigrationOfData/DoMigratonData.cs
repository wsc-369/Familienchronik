using app_familyBackend.DataContext;
using appAhnenforschungBackEnd.DataManager;
using appAhnenforschungData.Models.DB;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


//using ValueObject;
using static app_familyBackend.DataManager.AdressHelperMapper;
using static appAhnenforschungBackEnd.DataManager.CGlobal;
using static Entity.GenderStatusOfPerson;
//using Address = Entity.Address;

namespace app_familyBackend.MigrationOfData
{
  public class DoMigratonData(wsc_chronikContext SourceContext, MyDatabaseContext TargetContext)
  {
    private readonly MyDatabaseContext _targetContext = TargetContext;
    private readonly wsc_chronikContext _sourceContext = SourceContext;


    private DateTime ConvertOrDefault(object ticks)
    {
      var value = Convert.ToInt64(ticks);
      return value == 0 ? DateTime.MinValue : CGlobal.ConvertTicksToDateTime(value);
    }

    /// <summary>
    /// Reverts the migration of data related to persons by deleting all records from the associated tables within a
    /// transactional scope.
    /// </summary>
    /// <remarks>This method deletes all records from the "Partners", "Persons", and "PersonDetails" tables in
    /// the target database. The operation is performed within a transaction to ensure atomicity. If an exception
    /// occurs, the transaction is rolled back, and the exception is rethrown.</remarks>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns><see langword="true"/> if the operation completes successfully and the transaction is committed; otherwise, <see
    /// langword="false"/>.</returns>
    public async Task<bool> UndoMigratDataPersons(CancellationToken token)
    {
      bool result = false;
      using var transaction = await _targetContext.Database.BeginTransactionAsync(token);

      try
      {
        await _targetContext.ParentRoles.ExecuteDeleteAsync(token);
        await _targetContext.PartnerConnectionRoles.ExecuteDeleteAsync(token); 
        
        await _targetContext.PersonDetails.ExecuteDeleteAsync(token);
        await _targetContext.Partners.ExecuteDeleteAsync(token);
        await _targetContext.ParentChilds.ExecuteDeleteAsync(token);
        await _targetContext.Persons.ExecuteDeleteAsync(token);
        
        await DeletePersonRelations(token);
        //await _targetContext.PersonRelations.ExecuteDeleteAsync(token);
        await transaction.CommitAsync(token);
        result = true;
      }
      catch
      {
        await transaction.RollbackAsync(token);
        throw;
      }


      return result;
    }


    public async Task<bool> DoMigratUsers(CancellationToken token)
    {
      bool result = false;

      try
      {
        await _targetContext.Users.ExecuteDeleteAsync(token);
        var sourcePersons = await _sourceContext.TUsers.ToListAsync(token);

        foreach (var sp in sourcePersons)
        {

          Entity.Person person = null;
          if (!string.IsNullOrEmpty(sp.StrPersonId))
          {
            person = _targetContext.Persons.FirstOrDefault(x => x.PersonRefId == sp.StrPersonId);
          }

          var newUser = new User
          {
            Id = Guid.NewGuid(),
            UserId = sp.NUserId,
            PersonId = person != null ? person.Id : Guid.Empty,
            Salutation = sp.StrSalutation,
            Letter = sp.StrLetter,
            FamilyName = sp.StrName,
            FirstName = sp.StrPreName,
            Adress = sp.StrAdress,
            Zip = sp.NZip,
            Town = sp.StrTown,
            Country = sp.StrCountry,
            Email = sp.StrEmail,
            Tel = sp.StrTel,
            Remarks = sp.StrRemarks,
            Role = MapAppUserRoles(sp.NRole),
            AdmissionDate = sp.DtAdmissionDate ?? DateTime.MinValue,
            CheckOutDate = sp.DtCheckOutDate ?? DateTime.MinValue,
            LoginName = sp.StrLoginName?? String.Empty,
            Password = sp.StrPassword ?? String.Empty,
            HasPaid = sp.BHasPaid,
            PaidDate = sp.DtPaid ?? DateTime.MinValue,
            PersonAccessList = sp.StrPersonAccessList,
            Active = sp.BActive,
            AddTimestamp = DateTime.Now,
            UpdateTimestamp = DateTime.Now
          };
          _targetContext.Users.Add(newUser);
        }

        await _targetContext.SaveChangesAsync(token);


        result = true;
      }
      catch (Exception ex)
      {
        // await transaction.RollbackAsync(token);
        throw;
      }
      return result;
    }

    internal AppUserRoles MapAppUserRoles(int sourceRole)
    {
      return sourceRole switch
      {
        1 => AppUserRoles.Mitglied,
        2 => AppUserRoles.EditAdress,
        3 => AppUserRoles.EditMainPage,
        4 => AppUserRoles.Administrator,
        5 => AppUserRoles.EditDialect,
        _ => AppUserRoles.None,
      };
    }

   

    /// <summary>
    /// Migrates person-related data by cloning and merging records across multiple entities.
    /// </summary>
    /// <remarks>This method performs a transactional migration of person-related data, including cloning
    /// person and partner records and merging parent data into person records. If an error occurs during the migration,
    /// the transaction is rolled back, and the exception is propagated to the caller.</remarks>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns><see langword="true"/> if the migration completes successfully; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> DoMigratDataPersons(CancellationToken token)
    {
      bool result = false;
      using var transaction = await _targetContext.Database.BeginTransactionAsync(token);

      try
      {
        await DoMigratParentRoles();
        await DoMigratPartnerConnectionRoles();
        await ClonePerson(token);
        await ClonePartner(token);
        await MergeParentsToPerson(token);
        await CloneParentChild(token);
        await transaction.CommitAsync(token);

        result = true;
      }
      catch
      {
        await transaction.RollbackAsync(token);
        throw;
      }
      return result;
    }

    public async Task<bool> DoMigratPersonAddresses(CancellationToken token)
    {
      using var transaction = await _targetContext.Database.BeginTransactionAsync(token);
      try
      {

        await _targetContext.Addresses.ExecuteDeleteAsync(token);

        var sourceAddressess = await _sourceContext.TAdresses.ToListAsync(token);
        var sourceLegacyPersons = await _sourceContext.TPersons.Where(x => x.StrAdress.Length > 0).ToListAsync(token);
        var sourcePersons = await _targetContext.Persons.ToListAsync(token);
        foreach (var address in sourceAddressess)
        {
          if (sourcePersons.FirstOrDefault(x => x.PersonRefId == address.StrPersonId) == null)
          {
            continue;
          }
          var newAddress = new Entity.Address
          {
            Id = Guid.NewGuid(),
            PersonId = address.StrPersonId.Length > 0 ? sourcePersons.FirstOrDefault(x => x.PersonRefId == address.StrPersonId).Id : Guid.Empty,
            Street = address.StrAdresse ?? string.Empty,
            HouseNr = address.StrHouseNr ?? string.Empty,
            Town = address.StrTown ?? string.Empty,
            Zip = address.StrZip ?? string.Empty,
            Country = address.StrCountry ?? string.Empty,
            FullAddress = $"{RemoveSpaces(address.StrAdresse ?? string.Empty)} {RemoveSpaces(address.StrHouseNr ?? string.Empty)} {RemoveSpaces(address.StrZip ?? string.Empty)} {RemoveSpaces(address.StrTown ?? string.Empty)} {RemoveSpaces(address.StrCountry ?? string.Empty)}",
            OrderNr = address.NOrderNr!.Value,
            Description = address.StrAdresse ?? string.Empty,
            Active = address.BActive!.Value,
            AddTimestamp = address.DtCreate!.Value,
            UpdateTimestamp = address.DtUpdate!.Value
          };
          _targetContext.Addresses.Add(newAddress);
        }



        foreach (var addressLegacy in sourceLegacyPersons)
        {


          if (addressLegacy.StrAdress == "?")
          {
            continue;
          }
          if (sourcePersons.FirstOrDefault(x => x.PersonRefId == addressLegacy.StrPersonId) == null)
          {
            continue;
          }
          var addressParsed = AddressParser.ParseAddress(addressLegacy.StrAdress);


          //var town = objs.FirstOrDefault(x => adressLegacy.StrAdress.Contains(x.Town)) != null ? objs.FirstOrDefault(x => adressLegacy.StrAdress.Contains(x.Town)).Town : string.Empty;
          //var zip = objs.FirstOrDefault(x => adressLegacy.StrAdress.Contains(x.Zip)) != null ? objs.FirstOrDefault(x => adressLegacy.StrAdress.Contains(x.Zip)).Zip : string.Empty;
          //var country = objs.FirstOrDefault(x => adressLegacy.StrAdress.Contains(x.Country)) != null ? objs.FirstOrDefault(x => adressLegacy.StrAdress.Contains(x.Country)).Country : string.Empty;

          var newAddress = new Entity.Address
          {
            Id = Guid.NewGuid(),
            PersonId = addressLegacy.StrPersonId.Length > 0 ? sourcePersons.FirstOrDefault(x => x.PersonRefId == addressLegacy.StrPersonId).Id : Guid.Empty,
            Street = addressParsed?.Street ?? string.Empty,
            HouseNr = addressParsed?.HouseNumber ?? string.Empty,
            Town = addressParsed != null ? addressParsed.City : string.Empty,
            Zip = addressParsed?.PostalCode ?? string.Empty,
            Country = addressParsed?.Country ?? string.Empty,
            FullAddress = addressParsed != null ? $"{addressParsed.Street} {addressParsed.HouseNumber} {addressParsed.PostalCode} {addressParsed.City} {addressParsed.Country}" : addressLegacy.StrAdress,
            OrderNr = addressParsed != null ? -1 : -2,
            Description = addressLegacy?.StrAdress ?? string.Empty,
            Active = false,
            AddTimestamp = DateTime.Now,
            UpdateTimestamp = DateTime.Now,
          };
          _targetContext.Addresses.Add(newAddress);
        }

        await _targetContext.SaveChangesAsync(token);

        await transaction.CommitAsync(token);

        return true;// (IEnumerable<ValueObject.Person>)result.ToList();
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync(token);
        Console.WriteLine(ex.Message);
        return false;
      }
    }

    private string RemoveSpaces(string input)
    {
      return input != null ? input.Trim() : input;// new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
    }

    public async Task<bool> DoMigratDialectWordCollection(CancellationToken token)
    {
      bool result = false;

      try
      {
        await _targetContext.DialectWordCollection.ExecuteDeleteAsync(token);
        var sourcePersons = await _sourceContext.Tlemmas.ToListAsync(token);

        foreach (var sp in sourcePersons)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");
          if (!_targetContext.DialectWordCollection.Any(tp => tp.Title == sp.Title))
          {
            Entity.Person person = null;
            if (!string.IsNullOrEmpty(sp.StrPersonId))
            {
              person = _targetContext.Persons.FirstOrDefault(x => x.PersonRefId == sp.StrPersonId);
            }


            var newDialectWord = new Entity.DialectWord
            {
              Id = Guid.NewGuid(),
              PersonId = person != null ? person.Id : Guid.Empty,
              Title = sp.Title,
              Description = sp.Description,
              Voice = sp.Voice,
              Place = sp.Place,
              Source = sp.Source,
              Active = true,
              AddTimestamp = DateTime.Now,
              UpdateTimestamp = DateTime.Now
            };
            _targetContext.DialectWordCollection.Add(newDialectWord);
          }
        }
        await _targetContext.SaveChangesAsync(token);


        result = true;
      }
      catch
      {
        // await transaction.RollbackAsync(token);
        throw;
      }
      return result;
    }

    /// <summary>
    /// WIFI bezw. Personen auf dem Bild zuordnen.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> DoMigratWIFI(CancellationToken token)
    {
      using var transaction = await _targetContext.Database.BeginTransactionAsync(token);
      try
      {
        await _targetContext.PersonImages.ExecuteDeleteAsync(token);
        await _targetContext.ImagePersonPositions.ExecuteDeleteAsync(token);

        await this.MigrateWifiImagePerson(token);
        await this.MigrateWifiImagePersonPositions(token);
        await transaction.CommitAsync(token);

        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        await transaction.RollbackAsync(token);
        return false;
      }
    }

    public async Task<bool> DoMigratTemplates(CancellationToken token)
    {
      using var transaction = await _targetContext.Database.BeginTransactionAsync(token);
      try
      {
        await _targetContext.ContentTemplateLinks.ExecuteDeleteAsync(token);
        await _targetContext.ContentTemplateImages.ExecuteDeleteAsync(token);
        await _targetContext.ContentTemplates.ExecuteDeleteAsync(token);

        await this.MigrateContentTemplate(token);
        await this.MigrateContentTemplateImages(token);
        await this.MigrateContentTemplateLinks(token);
        await transaction.CommitAsync(token);

        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        await transaction.RollbackAsync(token);
        return false;
      }
    }

    /// <summary>
    /// Adds all person relations from the source context to the target context.
    /// </summary>
    /// <remarks>This method retrieves all persons from the source context and adds them to the target context
    /// if they do not already exist. Each person is mapped to a new entity with additional properties initialized based
    /// on the source data. The operation is performed asynchronously and respects the provided cancellation
    /// token.</remarks>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns><see langword="true"/> if the operation completes successfully; otherwise, <see langword="false"/>.</returns>
    internal async Task<bool> ClonePerson(CancellationToken token) //AddAllPersonRelations
    {
      try
      {
        var sourcePersons = await _sourceContext.TPersons.ToListAsync(token);

        foreach (var sp in sourcePersons)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");
          if (!_targetContext.Persons.Any(tp => tp.PersonRefId == sp.StrPersonId))
          {
            var newPerson = new Entity.Person
            {
              Id = Guid.NewGuid(),
              PersonRefId = sp.StrPersonId,
              FamilyName = sp.StrName,
              FirstName = sp.StrPreName,
              Status = sp.StrSex == "M" ? GenderStatus.Male : GenderStatus.Female,
              BirthPlace = string.Empty,
              DeathPlace = string.Empty,
              BurPlace = sp.StrBurPlace,
              BirthDate = ConvertOrDefault(sp.TikBirth),
              DeathDate = ConvertOrDefault(sp.TkDeath),
              BurDate = ConvertOrDefault(sp.TikBurDate),
              Race = sp.StrRace,
              Work = sp.StrWork,
              NameMerges = sp.StrMarriedName,
              Nickname = sp.StrNick,
              FatherId = Guid.Empty,
              MotherId = Guid.Empty,
              BirtYear = sp.NBirthYear.Value,
              DeathYear = sp.NDeathYear.Value,
              BirtMonth = sp.NBirthMonth.Value,
              DeathMonth = sp.NDeathMonth.Value,
              BirtDay = sp.NBirthDay.Value,
              DeathDay = sp.NDeathDay.Value,
              Active = sp.NActive == -1 ? true : false,
              AddTimestamp = DateTime.Now,
              UpdateTimestamp = DateTime.Now
            };
            _targetContext.Persons.Add(newPerson);
          }
        }
        await _targetContext.SaveChangesAsync(token);

        return true;// (IEnumerable<ValueObject.Person>)result.ToList();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    /// <summary>
    /// Clones partner data from the source context to the target context, associating partners with their corresponding
    /// persons.
    /// </summary>
    /// <remarks>This method retrieves partner data from the source context and maps it to the corresponding
    /// persons in the target context. If a person associated with a partner cannot be found in the target context, that
    /// partner is skipped. The method creates new partner records in the target context, setting their properties based
    /// on the source data.</remarks>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns><see langword="true"/> if the operation completes successfully; otherwise, <see langword="false"/>.</returns>
    internal async Task<bool> ClonePartner(CancellationToken token)
    {
      try
      {
        var persons = await _targetContext.Persons.ToListAsync(token);
        var sourcePartners = await _sourceContext.TPartners.ToListAsync(token);
        var byPersonId = persons.ToDictionary(p => p.PersonRefId);

        foreach (var spa in sourcePartners)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");

          if (!byPersonId.TryGetValue(spa.StrPersonId, out var person))
            continue; // falls nicht vorhanden, überspringen

          //var person = await _targetContext.Persons.SingleOrDefaultAsync(x => x.PersonID == sp.StrPersonId, token);
          var partnerState = spa.nConnectionType switch
          {
            (int)EPartnerConnectionRole.EhelichePartnerschaft => EPartnerConnectionRole.EhelichePartnerschaft,
            (int)EPartnerConnectionRole.NichtehelichePartnerschaft => EPartnerConnectionRole.NichtehelichePartnerschaft,
            (int)EPartnerConnectionRole.EingetragenePartnerschaft => EPartnerConnectionRole.EingetragenePartnerschaft,
            _ => EPartnerConnectionRole.EhelichePartnerschaft
          };


          var marriageDate = ConvertOrDefault(spa.TikMarriageDate);
          var divorceDate = ConvertOrDefault(spa.TikDivorceDate);

          var newPartner = new Entity.Partner
          {
            Id = Guid.NewGuid(),
            PersonId = person.Id,
            PartnerId = byPersonId.ContainsKey(spa.StrPartnerId) ? byPersonId[spa.StrPartnerId].Id : Guid.Empty,
            MarriageDateTime = marriageDate,
            DivorceDateTime = divorceDate,
            MarriageYear = marriageDate == DateTime.MinValue ? 0 : marriageDate.Year,
            DivorceYear = divorceDate == DateTime.MinValue ? 0 : divorceDate.Year,
            ConnectionRole = (int)partnerState,
            Active = true,
            AddTimestamp = DateTime.Now,
            UpdateTimestamp = DateTime.Now

          };
          _targetContext.Partners.Add(newPartner);

        }
        await _targetContext.SaveChangesAsync(token);

        return true;// (IEnumerable<ValueObject.Person>)result.ToList();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }


    
    //   internal async Task<bool> CloneParentChild(CancellationToken token)
    //{
    //  try
    //  {
    //    var persons = await _targetContext.Persons.ToListAsync(token);
    //    var sourcePartners = await _sourceContext.TPartners.ToListAsync(token);
    //    var byPersonId = persons.ToDictionary(p => p.PersonRefId);

    //    foreach (var spa in sourcePartners)
    //    {
    //      if (token.IsCancellationRequested)
    //        throw new OperationCanceledException("Token abgebrochen");

    //      if (!byPersonId.TryGetValue(spa.StrPersonId, out var person))
    //        continue; // falls nicht vorhanden, überspringen

    //      //var person = await _targetContext.Persons.SingleOrDefaultAsync(x => x.PersonID == sp.StrPersonId, token);
    //      var partnerState = spa.nConnectionType switch
    //      {
    //        (int)EPartnerConnectionRole.EhelichePartnerschaft => EPartnerConnectionRole.EhelichePartnerschaft,
    //        (int)EPartnerConnectionRole.NichtehelichePartnerschaft => EPartnerConnectionRole.NichtehelichePartnerschaft,
    //        (int)EPartnerConnectionRole.EingetragenePartnerschaft => EPartnerConnectionRole.EingetragenePartnerschaft,
    //        _ => EPartnerConnectionRole.EhelichePartnerschaft
    //      };


    //      var marriageDate = ConvertOrDefault(spa.TikMarriageDate);
    //      var divorceDate = ConvertOrDefault(spa.TikDivorceDate);

    //      var newPartner = new Entity.Partner
    //      {
    //        Id = Guid.NewGuid(),
    //        PersonId = person.Id,
    //        PartnerId = byPersonId.ContainsKey(spa.StrPartnerId) ? byPersonId[spa.StrPartnerId].Id : Guid.Empty,
    //        MarriageDateTime = marriageDate,
    //        DivorceDateTime = divorceDate,
    //        MarriageYear = marriageDate == DateTime.MinValue ? 0 : marriageDate.Year,
    //        DivorceYear = divorceDate == DateTime.MinValue ? 0 : divorceDate.Year,
    //        ConnectionRole = (int)partnerState,
    //        Active = true,
    //        AddTimestamp = DateTime.Now,
    //        UpdateTimestamp = DateTime.Now

    //      };
    //      _targetContext.Partners.Add(newPartner);

    //    }
    //    await _targetContext.SaveChangesAsync(token);

    //    return true;// (IEnumerable<ValueObject.Person>)result.ToList();
    //  }
    //  catch (Exception ex)
    //  {
    //    Console.WriteLine(ex.Message);
    //    return false;
    //  }
    //}

    internal async Task<bool> CloneParentChild(CancellationToken token)
    {
      try
      {
        var persons = await _targetContext.Persons.ToListAsync(token);
        foreach (var spa in persons)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");

          if (spa.FatherId == Guid.Empty && spa.MotherId == Guid.Empty)
            continue; // falls nicht vorhanden, überspringen

          var chils = persons.Where(x => x.FatherId == spa.Id);
          foreach (var c in chils)
          {
            var parent = persons.FirstOrDefault(x => x.Id == c.FatherId);
            if (parent == null) continue;
            var pc = new ParentChild
            {
              Id = Guid.NewGuid(),
              Child = c,
              Parent = parent,
              ParentRole = (int)EParentRole.Biological,
              Active = true,
              AddTimestamp = DateTime.Now,
              UpdateTimestamp = DateTime.Now
            };
            _targetContext.ParentChilds.Add(pc);
          }

            chils = persons.Where(x => x.MotherId == spa.Id);
            foreach (var c in chils)
            {
              var parent = persons.FirstOrDefault(x => x.Id == c.MotherId);
              if (parent == null) continue;
              var pc = new ParentChild
              {
                Id = Guid.NewGuid(),
                Child = c,
                Parent = parent,
                ParentRole = (int)EParentRole.Biological,
                Active = true,
                AddTimestamp = DateTime.Now,
                UpdateTimestamp = DateTime.Now
              };
            _targetContext.ParentChilds.Add(pc);
          }
        }

        await _targetContext.SaveChangesAsync(token);

        return true;;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
       throw ex;
      }
    }

    /// <summary>
    /// Merges parent information from the source context into the corresponding child records in the target context.
    /// </summary>
    /// <remarks>This method updates the parent-child relationships in the target context by mapping parent
    /// identifiers from the source context to their corresponding entities in the target context. If a parent is not
    /// found in the target context, the relationship is skipped. Changes are saved to the target context upon
    /// completion.</remarks>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns><see langword="true"/> if the operation completes successfully; otherwise, <see langword="false"/>.</returns>
    internal async Task<bool> MergeParentsToPerson(CancellationToken token)
    {
      try
      {
        var sourcePersons = await _sourceContext.TPersons.Where(x => x.StrFatherId != "-1" || x.StrMotherId != "-1").ToListAsync(token);
        var targetPersons = await _targetContext.Persons.ToListAsync(token);

        var byPersonId = targetPersons.ToDictionary(p => p.PersonRefId);
        foreach (var sp in sourcePersons)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");

          if (!byPersonId.TryGetValue(sp.StrPersonId, out var parent))
            continue; // Kind nicht gefunden

          if (sp.StrFatherId != "-1" && byPersonId.TryGetValue(sp.StrFatherId, out var father))
          {
            parent.FatherId = father.Id;
          }

          if (sp.StrMotherId != "-1" && byPersonId.TryGetValue(sp.StrMotherId, out var mother))
          {
            parent.MotherId = mother.Id;
          }
          _targetContext.Persons.Update(parent);
        }
        await _targetContext.SaveChangesAsync(token);

        return true;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    internal async Task<bool> MigrateWifiImagePerson(CancellationToken token)
    {
      try
      {
        var sources = await _sourceContext.TImagePersons.ToListAsync(token);

        foreach (var sp in sources)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");
          var newObject = new ImagePerson
          {
            Id = Guid.NewGuid(),
            RefImageId = sp.Id,
            Title = sp.Title,
            Description = sp.Description,
            PositionsCount = sp.PositionsCount,
            ImagePath = sp.ImagePath,
            FileName = sp.FileName,
            OriginalFileName = sp.OriginalFileName,
            SourceDescription = sp.SourceDescription,
            SourceImageFileName = sp.SourceImageFileName,
            State = this.MapImagePersonState(sp.Active.Value, sp.InProgress.Value, sp.IsArchivated.Value, sp.IsExported.Value),
            Active = true,
            AddTimestamp = sp.AddDate.Value,
            UpdateTimestamp = DateTime.Now
          };
          _targetContext.ImagePersons.Add(newObject);
        }
        await _targetContext.SaveChangesAsync(token);
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return false;
      }
    }

    internal ImagePersonState MapImagePersonState(bool isActive, bool inProgress, bool isArchivated, bool isExported)
    {
      var imagePersonState = ImagePersonState.None;

      if (isActive)
      {
        imagePersonState |= ImagePersonState.IsActive;
      }

      if (inProgress)
      {
        imagePersonState |= ImagePersonState.InProgress;
      }

      if (isArchivated)
      {
        imagePersonState |= ImagePersonState.IsArchivated;
      }

      if (isExported)
      {
        imagePersonState |= ImagePersonState.IsExported;
      }

      return imagePersonState;
    }

    internal async Task<bool> MigrateWifiImagePersonPositions(CancellationToken token)
    {
      try
      {
        var sources = await _sourceContext.TImagePersonsPositions.ToListAsync(token);

        foreach (var sp in sources)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");
          var personId = Guid.Empty;
          if (sp.ReferencePersonId.Length > 0 && _targetContext.Persons.Any(x => x.PersonRefId == sp.ReferencePersonId))
          {
            personId = _targetContext.Persons.FirstOrDefault(x => x.PersonRefId == sp.ReferencePersonId).Id;
          }
          var newObject = new ImagePersonPosition
          {
            Id = Guid.NewGuid(),
            ImagePersonId = _targetContext.ImagePersons.FirstOrDefault(x => x.RefImageId == sp.IdImagePerson).Id,
            ReferencePersonId = sp.ReferencePersonId,
            PersonId = personId,
            Pos = sp.Pos,
            FamilyName = sp.Name,
            FirstName = sp.PreName,
            Address = sp.Address,
            HouseNo = sp.HouseNo,
            Zip = sp.Zip,
            Country = sp.Country,
            Town = sp.Town,
            BirthYear = sp.BirthYear,
            Description = sp.Description,
            EditContactData = sp.EditContactData,
            EditEmail = sp.EditEmail,
            Finish = sp.Finish,
            Active = sp.Active,
            AddTimestamp = sp.AddDate,
            UpdateTimestamp = sp.UpdDate
          };

          _targetContext.ImagePersonPositions.Add(newObject);

        }
        await _targetContext.SaveChangesAsync(token);
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return false;
      }
    }

    internal async Task<bool> MigrateContentTemplate(CancellationToken token)
    {
      try
      {
        var sources = await _sourceContext.TContentTemplates.ToListAsync(token);

        foreach (var sp in sources)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");
          var newObject = new ContentTemplate
          {
            Id = Guid.NewGuid(),
            RefContentTemplateId = sp.NContentTemplateId,
            Title = sp.StrTitle,
            SubTitle = sp.StrSubTitle,
            Content = sp.StrContent,
            SortNo = sp.NSortNo.Value,
            Type = (CGlobal.TemplateTypes)sp.NType,
            Active = sp.NActive == -1 ? true : false,
            AddTimestamp = DateTime.MinValue,
            UpdateTimestamp = DateTime.MinValue
          };
          _targetContext.ContentTemplates.Add(newObject);

        }
        await _targetContext.SaveChangesAsync(token);
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return false;
      }
    }

    internal async Task<bool> MigrateContentTemplateImages(CancellationToken token)
    {
      try
      {
        var sources = await _sourceContext.TContentTemplateImages.ToListAsync(token);

        foreach (var sp in sources)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");
          var newObject = new ContentTemplateImage
          {
            Id = Guid.NewGuid(),
            ContentTemplateId = _targetContext.ContentTemplates.FirstOrDefault(x => x.RefContentTemplateId == sp.NContentTemplateId).Id,
            Title = sp.StrTitle,
            SubTitle = sp.StrSubTitle,
            ImageName = sp.StrImageName,
            ImageOriginalName = sp.StrImageOriginalName,
            Description = sp.StrDescription,
            SortNo = sp.NSortNo.Value,
            Active = sp.NActive == -1 ? true : false,
            AddTimestamp = DateTime.MinValue,
            UpdateTimestamp = DateTime.MinValue
          };
          _targetContext.ContentTemplateImages.Add(newObject);

        }
        await _targetContext.SaveChangesAsync(token);
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    internal async Task<bool> MigrateContentTemplateLinks(CancellationToken token)
    {
      try
      {
        var sources = await _sourceContext.TContentTemplateLinks.ToListAsync(token);

        foreach (var sp in sources)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");
          var newObject = new ContentTemplateLink
          {
            Id = Guid.NewGuid(),
            ContentTemplateId = _targetContext.ContentTemplates.FirstOrDefault(x => x.RefContentTemplateId == sp.NContentTemplateId).Id,
            Title = sp.StrTitle,
            SubTitle = sp.StrSubTitle,
            IsExternalLink = sp.ExternalLink == -1 ? true : false,
            NavigationTo = sp.StrNavigationTo,
            PersonId = sp.StrPersonId.Length > 0 ? _targetContext.Persons.FirstOrDefault(x => x.PersonRefId == sp.StrPersonId).Id : Guid.Empty,
            Description = sp.StrDescription,
            SortNo = sp.NSortNo.Value,
            Active = sp.NActive == -1 ? true : false,
            AddTimestamp = DateTime.MinValue,
            UpdateTimestamp = DateTime.MinValue
          };
          _targetContext.ContentTemplateLinks.Add(newObject);

        }
        await _targetContext.SaveChangesAsync(token);
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    internal async Task<bool> DeletePersons(CancellationToken token)
    {
      try
      {
        int offset = 0;
        while (true)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");

          var query = _targetContext.Persons.Take(5000).Skip(offset);

          if (!query.Any())
            break;

          foreach (var item in query)
          {
            _targetContext.Persons.Remove(item);
          }

          await _targetContext.SaveChangesAsync();

          offset += 5000;
        }

        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        // Log.Error(ex); // oder weitere Logging-Funktion
        throw;
      }
    }

    internal async Task<bool> DeletePersonRelations(CancellationToken token)
    {
      try
      {
        int offset = 0;
        while (true)
        {
          if (token.IsCancellationRequested)
            throw new OperationCanceledException("Token abgebrochen");

          var query = _targetContext.PersonRelations.Take(5000).Skip(offset);

          if (!query.Any())
            break;

          foreach (var item in query)
          {
            _targetContext.PersonRelations.Remove(item);
          }

          await _targetContext.SaveChangesAsync();

          offset += 5000;
        }

        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        // Log.Error(ex); // oder weitere Logging-Funktion
        throw;
      }
    }

    internal async Task<bool> DoMigratParentRoles()
    {
      bool result = false;

      try
      {
        var newParentRole = new ParentRole
        {
          Id = Guid.NewGuid(),
          Role= (int)EParentRole.Biological,
          Name = "biologisch",
          Description = "Der Elternteil ist genetisch mit dem Kind verwandt. Dies entspricht dem leiblichen Vater oder der leiblichen Mutter.",
          Active = true
        };
        _targetContext.ParentRoles.Add(newParentRole);

        newParentRole = new ParentRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EParentRole.Adoptive,
          Name = "adoptiert",
          Description = "Der Elternteil hat das Kind rechtlich adoptiert. Es besteht eine gesetzliche Eltern‑Kind‑Beziehung ohne genetische Abstammung.",
          Active = true
        };
        _targetContext.ParentRoles.Add(newParentRole);


        newParentRole = new ParentRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EParentRole.Step,
          Name = "stief",
          Description = "Der Elternteil übernimmt eine elterliche Rolle im Alltag des Kindes, ohne biologische, rechtliche oder eheliche Verbindung. Beispiele: langjähriger Lebenspartner eines Elternteils, wichtige Bezugsperson.",
          Active = true
        };
        _targetContext.ParentRoles.Add(newParentRole);

        newParentRole = new ParentRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EParentRole.Foster,
          Name = "pflege",
          Description = "Der Elternteil betreut das Kind im Rahmen einer Pflegefamilie. Es besteht weder eine biologische noch eine rechtliche Elternschaft.",
          Active = true
        };
        _targetContext.ParentRoles.Add(newParentRole);

        newParentRole = new ParentRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EParentRole.Social,
          Name = "sozial",
          Description = "Der Elternteil übernimmt eine elterliche Rolle im Alltag des Kindes, ohne biologische, rechtliche oder eheliche Verbindung. Beispiele: langjähriger Lebenspartner eines Elternteils, wichtige Bezugsperson.",
          Active = true
        };
        _targetContext.ParentRoles.Add(newParentRole);

        newParentRole = new ParentRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EParentRole.Legal,
          Name = "rechtlich",
          Description = "Der Elternteil ist gesetzlich als Elternteil anerkannt (z. B. durch Gerichtsbeschluss oder Eintrag in der Geburtsurkunde), unabhängig von der biologischen Abstammung.",
          Active = true
        };
        _targetContext.ParentRoles.Add(newParentRole);

        newParentRole = new ParentRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EParentRole.Intended,
          Name = "intendiert",
          Description = "Der Elternteil ist im Rahmen von Reproduktionsmedizin oder Leihmutterschaft als zukünftiger rechtlicher Elternteil vorgesehen.",
          Active = true
        };
        _targetContext.ParentRoles.Add(newParentRole);

        newParentRole = new ParentRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EParentRole.Guardian,
          Name = "vormund",
          Description = "Eine vom Gericht eingesetzte oder gesetzlich bestimmte Person, die für das Wohl des Kindes verantwortlich ist, ohne Elternteil im engeren Sinne zu sein.",
          Active = true
        };
        _targetContext.ParentRoles.Add(newParentRole);

        newParentRole = new ParentRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EParentRole.Unknown,
          Name = "unbekannt",
          Description = "Der Elternteil ist nicht bekannt oder nicht dokumentiert. Nützlich für unvollständige genealogische Daten.",
          Active = true
        };
        _targetContext.ParentRoles.Add(newParentRole);

        await _targetContext.SaveChangesAsync();

        result = true;
      }
      catch (Exception ex)
      {
        // await transaction.RollbackAsync(token);
        throw;
      }
      return result;
    }

    internal async Task<bool> DoMigratPartnerConnectionRoles()
    {
      bool result = false;

      try
      {
        var newParentRole = new PartnerConnectionRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EPartnerConnectionRole.EhelichePartnerschaft,
          Name = EPartnerConnectionRole.EhelichePartnerschaft.ToString(),
          Active = true
        };
        _targetContext.PartnerConnectionRoles.Add(newParentRole);

        newParentRole = new PartnerConnectionRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EPartnerConnectionRole.EingetragenePartnerschaft,
          Name = EPartnerConnectionRole.EingetragenePartnerschaft.ToString(),
          Active = true
        };
        _targetContext.PartnerConnectionRoles.Add(newParentRole);

        newParentRole = new PartnerConnectionRole
        {
          Id = Guid.NewGuid(),
          Role = (int)EPartnerConnectionRole.NichtehelichePartnerschaft,
          Name = EPartnerConnectionRole.NichtehelichePartnerschaft.ToString(),
          Active = true
        };
        _targetContext.PartnerConnectionRoles.Add(newParentRole);

        await _targetContext.SaveChangesAsync();

        result = true;
      }
      catch (Exception ex)
      {
        // await transaction.RollbackAsync(token);
        throw;
      }
      return result;
    }
  }
}