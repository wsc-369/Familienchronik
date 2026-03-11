using app_familyBackend.DataContext;
using appAhnenforschungBackEnd.Filters;
using appAhnenforschungData.DataManager;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValueObject;


namespace app_familyChronikApi.ReadWriteDB
{
  public class ReadPersonRelations(MyDatabaseContext context)
  {
    private readonly MyDatabaseContext _context = context;


    internal int CoumutePersonOlder(ValueObject.Person rel)
    {
      TimeSpan ts;
      if (rel.BirthDate != DateTime.MinValue && rel.DeathDate == DateTime.MinValue)
      {
        ts = DateTime.Now - rel.BirthDate;
        return (int)ts.TotalDays / 365;
      }

      else if (rel.BirthDate != DateTime.MinValue && rel.DeathDate != DateTime.MinValue)
      {
        ts = rel.DeathDate - rel.BirthDate;
        return (int)ts.TotalDays / 365;
      }
      else
      {
        return 0;
      }

    }

    private void AddToList(ref List<ValueObject.Person> arlPerson, ValueObject.Person rel)
    {
      if (!arlPerson.Any(element => element.Id == rel.Id))
      {
        rel.Older = CoumutePersonOlder(rel);
        arlPerson.Add(rel);
      }
    }

    public async Task<IEnumerable<ValueObject.Person>> GetPersonWildcardFilterByPersonId(FilterPersons oFilterPersons, CSettings i_oSettings)
    {
      try
      {
        List<ValueObject.Person> arlPerson = new List<ValueObject.Person>();
        if (oFilterPersons.personID != null && oFilterPersons.personID.Length >= 1)
        {
          //oFilterPersons.personID = oFilterPersons.personID.Replace("*", "");
          //var p = await _context.Persons.Where(t => t.PersonID == oFilterPersons.personID).ToListAsync(token);
          var p = await _context.Persons.FirstOrDefaultAsync(t => t.PersonRefId == oFilterPersons.personID);
          var rel = await _context.PersonRelations.FirstOrDefaultAsync(t => t.Id == p.Id);
          AddToList(ref arlPerson, rel.Person);
        }
        return arlPerson;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public async Task<IEnumerable<ValueObject.TypeheadPerson>> GetPersonByQuery(string familename, string firstname, CancellationToken token)
    {
      List<ValueObject.TypeheadPerson> arlNames = new List<ValueObject.TypeheadPerson>();

      var result = await _context.Persons.Where(t => t.FamilyName.Contains(familename) && t.FirstName.Contains(firstname)).ToListAsync(token);
      foreach (var person in result)
      {
        var p = new TypeheadPerson(person.Id, person.FamilyName, person.FirstName, person.BirthDate);

        arlNames.Add(p);
      }

      return arlNames;
    }



    public async Task<IEnumerable<ValueObject.Person>> GetPersonWildcardFilter(FilterPersons oFilterPersons, CSettings i_oSettings)
    {
      try
      {
        List<ValueObject.Person> arlPerson = new List<ValueObject.Person>();
        if (oFilterPersons.personID != null && oFilterPersons.personID.Length >= 1)
        {
          //      //oFilterPersons.personID = oFilterPersons.personID.Replace("*", "");
          //      //var p = await _context.Persons.Where(t => t.PersonID == oFilterPersons.personID).ToListAsync(token);
          var p = await _context.Persons.SingleOrDefaultAsync(t => t.PersonRefId == oFilterPersons.personID);
          AddToList(ref arlPerson, await GetPersonAsync(p));

          //var rel = await _context.PersonRelations.SingleOrDefaultAsync(t => t.Id == p.Id, token);
          //if (rel != null)
          //{
          //  AddToList(ref arlPerson, rel.Person);
          //}
        }

        if (oFilterPersons.firstName != null && oFilterPersons.firstName.Length >= 1 && oFilterPersons.familyName != null && oFilterPersons.familyName.Length >= 1)
        {

          var p = await _context.Persons.Where(t => t.FirstName.Contains(oFilterPersons.firstName) && t.FamilyName.Contains(oFilterPersons.familyName)).ToListAsync();
          foreach (var person in p)
          {
            AddToList(ref arlPerson, await GetPersonAsync(person));
          }
        }

        if (oFilterPersons.firstName != null && oFilterPersons.firstName.Length >= 1 && oFilterPersons.familyName == null)
        {

          var p = await _context.Persons.Where(t => t.FirstName.Contains(oFilterPersons.firstName)).ToListAsync();
          foreach (var person in p)
          {
            AddToList(ref arlPerson, await GetPersonAsync(person));
          }
        }

        if (oFilterPersons.firstName == null && oFilterPersons.familyName != null && oFilterPersons.familyName.Length >= 1)
        {

          var p = await _context.Persons.Where(t => t.FamilyName.Contains(oFilterPersons.familyName)).ToListAsync();
          foreach (var person in p)
          {
            AddToList(ref arlPerson, await GetPersonAsync(person));
          }
        }


        if (oFilterPersons.birthDate != null && oFilterPersons.birthDate != "undefined")
        {
          DateTime dtBirt = DateTime.Parse(oFilterPersons.birthDate);

          var p = await _context.Persons.Where(t => t.BirtMonth == dtBirt.Month && t.BirtDay == dtBirt.Day).ToListAsync();
          foreach (var person in p)
          {
            AddToList(ref arlPerson, await GetPersonAsync(person));
          }
        }

        if (oFilterPersons.deathDate != null && oFilterPersons.deathDate != "undefined")
        {
          DateTime deathDate = DateTime.Parse(oFilterPersons.deathDate);

          var p = await _context.Persons.Where(t => t.DeathMonth == deathDate.Month && t.DeathDay == deathDate.Day).ToListAsync();
          foreach (var person in p)
          {
            AddToList(ref arlPerson, await GetPersonAsync(person));
          }
        }

        if (oFilterPersons.birthYear != null && oFilterPersons.birthYear != "undefined" && Convert.ToInt32(oFilterPersons.birthYear) > 0)
        {
          var p = await _context.Persons.Where(t => t.BirtYear == Convert.ToInt32(oFilterPersons.birthYear)).ToListAsync();
          foreach (var person in p)
          {
            AddToList(ref arlPerson, await GetPersonAsync(person));
          }
        }

        if (oFilterPersons.older != null && oFilterPersons.older != "undefined" && Convert.ToInt32(oFilterPersons.older) > 0)
        {
          DateTime dtBirthYear = DateTime.Now.AddYears(-Convert.ToInt32(oFilterPersons.older));

          var p = await _context.Persons.Where(t => t.BirtYear == dtBirthYear.Year && t.DeathYear == 0).ToListAsync();
          foreach (var person in p)
          {
            AddToList(ref arlPerson, await GetPersonAsync(person));
          }
        }

        if (oFilterPersons.dateFrom != null && oFilterPersons.dateFrom != "undefined" && oFilterPersons.dateUntil != null && oFilterPersons.dateUntil != "undefined")
        {
          DateTime dateFrom = DateTime.Parse(oFilterPersons.dateFrom);
          DateTime dateUntil = DateTime.Parse(oFilterPersons.dateUntil);

          var p = await _context.Persons.Where(t => t.BirthDate.Ticks >= Convert.ToInt64(dateFrom.Ticks) &&
            Convert.ToInt64(t.BirthDate) <= Convert.ToInt64(dateUntil.Ticks)).ToListAsync();
          foreach (var person in p)
          {
            AddToList(ref arlPerson, await GetPersonAsync(person));
          }
        }


        if (oFilterPersons.wildCardText != null && oFilterPersons.wildCardText.Length >= 1)
        {
          var preNamePattern = $"%\"FamliyName\":\"{oFilterPersons.wildCardText}%\"%";
          var firstNamePattern = $"%\"FirstName\":\"%{oFilterPersons.wildCardText}%\"%";
          var nameMergesPattern = $"%\"NameMerges\":\"%{oFilterPersons.wildCardText}%\"%";
          var racePattern = $"%\"Race\":\"%{oFilterPersons.wildCardText}%\"%";
          var burPlacePattern = $"%\"BurPlace\":\"%{oFilterPersons.wildCardText}%\"%";
          var birthPlacePattern = $"%\"BirthPlace\":\"%{oFilterPersons.wildCardText}%\"%";
          var deathPlacePattern = $"%\"DeathPlace\":\"%{oFilterPersons.wildCardText}%\"%";
          var workPattern = $"%\"Work\":\"%{oFilterPersons.wildCardText}%\"%";
          var nicknamePattern = $"%\"Nickname\":\"%{oFilterPersons.wildCardText}%\"%";

          var personRelations = await _context.PersonRelations
              .Where(pr =>
                  EF.Functions.Like(pr.Value, preNamePattern)
                    || EF.Functions.Like(pr.Value, firstNamePattern)
                    || EF.Functions.Like(pr.Value, nameMergesPattern)
                    || EF.Functions.Like(pr.Value, racePattern)
                    || EF.Functions.Like(pr.Value, burPlacePattern)
                    || EF.Functions.Like(pr.Value, birthPlacePattern)
                    || EF.Functions.Like(pr.Value, deathPlacePattern)
                    || EF.Functions.Like(pr.Value, workPattern)
                    || EF.Functions.Like(pr.Value, nicknamePattern))
              .ToListAsync();

          foreach (var rel in personRelations)
          {
            AddToList(ref arlPerson, rel.Person);
          }
        }

        //if (oFilterPersons.firstName != null && oFilterPersons.firstName.Length >= 1 && oFilterPersons.preName != null && oFilterPersons.preName.Length >= 1)
        //{
        //  var preNamePattern = $"%\"PreName\":\"{oFilterPersons.firstName}%\"%";
        //  var firstNamePattern = $"%\"FirstName\":\"%{oFilterPersons.preName}%\"%";

        //  var personRelations = await _context.PersonRelations
        //      .Where(pr =>
        //          EF.Functions.Like(pr.Value, preNamePattern) && EF.Functions.Like(pr.Value, firstNamePattern))
        //      .ToListAsync(token);

        //  foreach (var rel in personRelations)
        //  {
        //    AddToList(ref arlPerson, rel.Person);
        //  }
        //}
        return arlPerson.OrderByDescending(x=> x.BirthDate) ;
      }
      catch (Exception)
      {
        throw;
      }
      //    else if (oFilterPersons.firstName != null && oFilterPersons.firstName.Length >= 1
      //       && oFilterPersons.preName != null && oFilterPersons.preName.Length >= 1)
      //    {

      //      //var personRelations = await _context.PersonRelations
      //      //.Where(pr =>
      //      //    EF.Functions.Like(pr.Value, $"%\"PreName\":\"{oFilterPersons.firstName}%\"%") ||
      //      //    EF.Functions.Like(pr.Value, $"%\"FirstName\":\"{oFilterPersons.firstName}%\"%"))// ||
      //         // EF.Functions.Like(pr.Value, $"%\"Nickname\":\"{searchNickname}%\"%"))
      //      //.ToListAsync();
      //      // var persons = await _context.Persons.Where(x=> personRelations.Contains(x.Id).til;
      //      //foreach (DataModel.PersonRelation rel in personRelations)
      //      //{
      //      //  var person = await _context.Persons.FirstOrDefaultAsync(t => t.Id == rel.Id, token);
      //      //  //AddToList(ref arlPerson, rel.Person);
      //      //}

      //      //// Ends
      //      //if (oFilterPersons.firstName.EndsWith("*") && oFilterPersons.preName.EndsWith("*"))
      //      //{
      //      //  oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
      //      //  oFilterPersons.preName = oFilterPersons.preName.Replace("*", "");

      //      //  var persons = await _context.Persons.Where(t => t.Familyname.StartsWith(oFilterPersons.preName) && t.FirstName.StartsWith(oFilterPersons.firstName)).ToListAsync(token);

      //      //  foreach (appAhnenforschungData.DataModel.Person person in persons)
      //      //  {
      //      //    var rel = await _context.PersonRelations.FirstOrDefaultAsync(t => t.Id == person.Id, token);
      //      //    AddToList(ref arlPerson, rel.Person);
      //      //  }
      //      }
      //      else if (oFilterPersons.firstName.StartsWith("*") && oFilterPersons.preName.StartsWith("*"))
      //      {
      //        //oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
      //        //oFilterPersons.preName = oFilterPersons.preName.Replace("*", "");

      //        //var persons = await _context.Persons.Where(t => t.Familyname.EndsWith(oFilterPersons.preName) && t.FirstName.EndsWith(oFilterPersons.firstName)).ToListAsync(token);

      //        //foreach (appAhnenforschungData.DataModel.Person person in persons)
      //        //{
      //        //  var rel = await _context.PersonRelations.FirstOrDefaultAsync(t => t.Id == person.Id, token);
      //        //  AddToList(ref arlPerson, rel.Person);
      //        //}
      //      }
      //      else if (oFilterPersons.firstName.EndsWith("*") && oFilterPersons.preName.StartsWith("*"))
      //      {
      //        //oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
      //        //oFilterPersons.preName = oFilterPersons.preName.Replace("*", "");

      //        //var persons = await _context.Persons.Where(t => t.Familyname.StartsWith(oFilterPersons.preName) && t.FirstName.EndsWith(oFilterPersons.firstName)).ToListAsync(token);

      //        //foreach (appAhnenforschungData.DataModel.Person person in persons)
      //        //{
      //        //  var rel = await _context.PersonRelations.FirstOrDefaultAsync(t => t.Id == person.Id, token);
      //        //  AddToList(ref arlPerson, rel.Person);
      //        //}
      //      }
      //      else if (oFilterPersons.firstName.StartsWith("*") && oFilterPersons.preName.EndsWith("*"))
      //      {
      //        //oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
      //        //oFilterPersons.preName = oFilterPersons.preName.Replace("*", "");

      //        //var persons = await _context.Persons.Where(t => t.Familyname.EndsWith(oFilterPersons.preName) && t.FirstName.StartsWith(oFilterPersons.firstName)).ToListAsync(token);

      //        //foreach (appAhnenforschungData.DataModel.Person person in persons)
      //        //{
      //        //  var rel = await _context.PersonRelations.FirstOrDefaultAsync(t => t.Id == person.Id, token);
      //        //  AddToList(ref arlPerson, rel.Person);
      //        //}
      //      }
      //      else if (oFilterPersons.firstName.StartsWith("*"))
      //      {
      //        //oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");

      //        //var persons = await _context.Persons.Where(t => t.Familyname.EndsWith(oFilterPersons.preName) && t.FirstName.EndsWith(oFilterPersons.firstName)).ToListAsync(token);

      //        //foreach (appAhnenforschungData.DataModel.Person person in persons)
      //        //{
      //        //  var rel = await _context.PersonRelations.FirstOrDefaultAsync(t => t.Id == person.Id, token);
      //        //  AddToList(ref arlPerson, rel.Person);
      //        //}
      //      }
      //      else if (oFilterPersons.firstName.EndsWith("*"))
      //      {
      //        //oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");

      //        //var persons = await _context.Persons.Where(t => t.Familyname == oFilterPersons.preName && t.FirstName.StartsWith(oFilterPersons.firstName)).ToListAsync(token);

      //        //foreach (appAhnenforschungData.DataModel.Person person in persons)
      //        //{
      //        //  var rel = await _context.PersonRelations.FirstOrDefaultAsync(t => t.Id == person.Id, token);
      //        //  AddToList(ref arlPerson, rel.Person);
      //        //}
      //      }
      //      else if (oFilterPersons.preName.StartsWith("*"))
      //      {
      //        //oFilterPersons.preName = oFilterPersons.preName.Replace("*", "");

      //        //oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");

      //        //var persons = await _context.Persons.Where(t => t.Familyname.EndsWith(oFilterPersons.preName) && t.FirstName == oFilterPersons.firstName).ToListAsync(token);

      //        //foreach (appAhnenforschungData.DataModel.Person person in persons)
      //        //{
      //        //  var rel = await _context.PersonRelations.FirstOrDefaultAsync(t => t.Id == person.Id, token);
      //        //  AddToList(ref arlPerson, rel.Person);
      //        //}
      //      }
      //      else if (oFilterPersons.preName.EndsWith("*"))
      //      {
      //        //oFilterPersons.preName = oFilterPersons.preName.Replace("*", "");
      //        //foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.StartsWith(oFilterPersons.preName) && t.StrPreName == oFilterPersons.firstName))
      //        //{
      //        //  CPerson oPerson = new CPerson();
      //        //  MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
      //        //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //        //  {
      //        //    arlPerson.Add(oPerson);
      //        //  }
      //        //}
      //      }
      //      else
      //      {
      //        //foreach (TPerson tperson in db.TPersons.Where(t => t.StrName == oFilterPersons.preName && t.StrPreName == oFilterPersons.firstName))
      //        //{
      //        //  CPerson oPerson = new CPerson();
      //        //  MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
      //        //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //        //  {
      //        //    arlPerson.Add(oPerson);
      //        //  }
      //        //}
      //      }
      //    }
      //    else if (oFilterPersons.firstName != null && oFilterPersons.firstName.Length >= 1 && oFilterPersons.preName == null)
      //    {
      //      if (oFilterPersons.firstName.StartsWith("*"))
      //      {
      //        //oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
      //        //foreach (TPerson tperson in db.TPersons.Where(t => t.StrName == oFilterPersons.preName && t.StrPreName.EndsWith(oFilterPersons.firstName)))
      //        //{
      //        //  CPerson oPerson = new CPerson();
      //        //  MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
      //        //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //        //  {
      //        //    arlPerson.Add(oPerson);
      //        //  }
      //        //}

      //      }
      //      else if (oFilterPersons.firstName.EndsWith("*"))
      //      {
      //        //oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
      //        //foreach (TPerson tperson in db.TPersons.Where(t => t.StrPreName.StartsWith(oFilterPersons.firstName)))
      //        //{
      //        //  CPerson oPerson = new CPerson();
      //        //  MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
      //        //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //        //  {
      //        //    arlPerson.Add(oPerson);
      //        //  }
      //        //}
      //      }
      //      else
      //      {
      //        //foreach (TPerson tperson in db.TPersons.Where(t => t.StrPreName == oFilterPersons.firstName))
      //        //{
      //        //  CPerson oPerson = new CPerson();
      //        //  MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
      //        //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //        //  {
      //        //    arlPerson.Add(oPerson);
      //        //  }
      //        //}
      //      }
      //    }
      //    else if (oFilterPersons.preName != null && oFilterPersons.preName.Length >= 1 && oFilterPersons.firstName == null)
      //    {
      //      if (oFilterPersons.preName.StartsWith("*"))
      //      {
      //        //oFilterPersons.preName = oFilterPersons.preName.Replace("*", "");
      //        //foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.EndsWith(oFilterPersons.preName)))
      //        //{
      //        //  CPerson oPerson = new CPerson();
      //        //  MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
      //        //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //        //  {
      //        //    arlPerson.Add(oPerson);
      //        //  }
      //        //}
      //      }
      //      else if (oFilterPersons.preName.EndsWith("*"))
      //      {
      //        //oFilterPersons.preName = oFilterPersons.preName.Replace("*", "");
      //        //foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.StartsWith(oFilterPersons.preName)))
      //        //{
      //        //  CPerson oPerson = new CPerson();
      //        //  MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
      //        //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //        //  {
      //        //    arlPerson.Add(oPerson);
      //        //  }
      //        //}
      //      }
      //      else
      //      {
      //        //foreach (TPerson tperson in db.TPersons.Where(t => t.StrName == oFilterPersons.preName))
      //        //{
      //        //  CPerson oPerson = new CPerson();
      //        //  MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
      //        //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //        //  {
      //        //    arlPerson.Add(oPerson);
      //        //  }
      //        //}
      //      }
      //    }

      //    bool OnlyFilterDate = false;
      //    // Exist filters
      //    if (oFilterPersons.preName == null && oFilterPersons.firstName == null)
      //    {
      //      OnlyFilterDate = true;
      //    }
      //    else if (oFilterPersons.preName == null && oFilterPersons.firstName != null)
      //    {
      //      if (oFilterPersons.firstName.Length == 0)
      //      {
      //        OnlyFilterDate = true;
      //      }
      //    }
      //    else if (oFilterPersons.preName != null && oFilterPersons.firstName == null)
      //    {
      //      if (oFilterPersons.preName.Length == 0)
      //      {
      //        OnlyFilterDate = true;
      //      }
      //    }

      //    if (oFilterPersons.birthDate != null && oFilterPersons.birthDate != "undefined")
      //    {
      //      DateTime dtBirt = DateTime.Parse(oFilterPersons.birthDate);
      //      //if (arlPerson.Count == 0 && OnlyFilterDate == true)
      //      //{
      //      //  //var tPersons = db.TPersons.Where(t => t.NBirthMonth == dtBirt.Month && t.NBirthDay == dtBirt.Day).ToList();
      //      //  //var personIds = arlPerson.Select(p => p.PersonID).ToHashSet();

      //      //  //foreach (var person in tPersons)
      //      //  //{
      //      //  //  var oPerson = new CPerson();
      //      //  //  MappPersonEntityToModel(ref oPerson, person, tPersons, i_oSettings);

      //      //  //  if (!personIds.Contains(oPerson.PersonID))
      //      //  //  {
      //      //  //    arlPerson.Add(oPerson);
      //      //  //    personIds.Add(oPerson.PersonID); // Hinzufügen des neuen PersonIDs
      //      //  //  }
      //      //  //}
      //      //}
      //      //else
      //      //{
      //      //  arlPerson = arlPerson.Where(t => t.BirthDate.Month == dtBirt.Month && t.BirthDate.Day == dtBirt.Day).ToList();
      //      //}

      //    }
      //    if (oFilterPersons.deathDate != null && oFilterPersons.deathDate != "undefined")
      //    {
      //      //DateTime dtDeath = DateTime.Parse(oFilterPersons.deathDate);
      //      //if (arlPerson.Count == 0 && OnlyFilterDate == true)
      //      //{
      //      //  //var tPersons = db.TPersons.Where(t => t.NDeathMonth == dtDeath.Month && t.NDeathDay == dtDeath.Day).ToList();
      //      //  //foreach (TPerson tperson in tPersons)
      //      //  //{
      //      //  //  CPerson oPerson = new CPerson();
      //      //  //  MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
      //      //  //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //      //  //  {
      //      //  //    arlPerson.Add(oPerson);
      //      //  //  }
      //      //  //}
      //      //}
      //      //else
      //      //{
      //      //  arlPerson = arlPerson.Where(t => t.DeathDate.Month == dtDeath.Month && t.DeathDate.Day == dtDeath.Day).ToList();
      //      //}
      //    }


      //    if (oFilterPersons.birthYear != null && oFilterPersons.birthYear != "undefined" && Convert.ToInt32(oFilterPersons.birthYear) > 0)
      //    {
      //      //if (arlPerson.Count == 0 && OnlyFilterDate == true)
      //      //{
      //      //  //var tPersons = db.TPersons.Where(t => t.NBirthYear == Convert.ToInt32(oFilterPersons.birthYear)).ToList();
      //      //  //foreach (TPerson tperson in tPersons)
      //      //  //{
      //      //  //  CPerson oPerson = new CPerson();
      //      //  //  MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
      //      //  //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //      //  //  {
      //      //  //    arlPerson.Add(oPerson);
      //      //  //  }
      //      //  //}
      //      //}
      //      //else
      //      //{
      //      //  arlPerson = arlPerson.Where(t => t.BirthDate.Year == Convert.ToInt32(oFilterPersons.birthYear)).ToList();
      //      //}
      //    }


      //    if (oFilterPersons.older != null && oFilterPersons.older != "undefined" && Convert.ToInt32(oFilterPersons.older) > 0)
      //    {
      //      //DateTime dtBirthYear = DateTime.Now.AddYears(-Convert.ToInt32(oFilterPersons.older));
      //      //if (arlPerson.Count == 0 && OnlyFilterDate == true)
      //      //{
      //      //  //var tPersons = db.TPersons.Where(t => t.NBirthYear == dtBirthYear.Year && t.TkDeath == "0").ToList();
      //      //  //foreach (TPerson tperson in tPersons)
      //      //  //{
      //      //  //  CPerson oPerson = new CPerson();
      //      //  //  MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
      //      //  //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //      //  //  {
      //      //  //    arlPerson.Add(oPerson);
      //      //  //  }
      //      //  //}
      //      //}
      //      //else
      //      //{
      //      //  //arlPerson = arlPerson.Where(t => t.Older == Convert.ToInt32(oFilterPersons.older)).ToList();
      //      //}
      //    }


      //    if (oFilterPersons.dateFrom != null && oFilterPersons.dateFrom != "undefined" && oFilterPersons.dateUntil != null && oFilterPersons.dateUntil != "undefined")
      //    {
      //      DateTime dateFrom = DateTime.Parse(oFilterPersons.dateFrom);
      //      DateTime dateUntil = DateTime.Parse(oFilterPersons.dateUntil);

      //      //var tPersons = db.TPersons.Where(t => Convert.ToInt64(t.TikBirth) >= Convert.ToInt64(dateFrom.Ticks)
      //      //                                                && Convert.ToInt64(t.TikBirth) <= Convert.ToInt64(dateUntil.Ticks)).ToList();
      //      //foreach (TPerson tperson in tPersons)
      //      //{
      //      //  CPerson oPerson = new CPerson();
      //      //  MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
      //      //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //      //  {
      //      //    arlPerson.Add(oPerson);
      //      //  }
      //      //}
      //    }


      //    else if (oFilterPersons.wildCardText != null && oFilterPersons.wildCardText.Length >= 1)
      //    {
      //      //var tPersons = db.TPersons.ToList();
      //      //var tRemark = db.TRemarks.Where(t => t.StrRemarks.Contains(oFilterPersons.wildCardText) || t.StrRemarksClean.Contains(oFilterPersons.wildCardText)).ToList();
      //      //foreach (TRemark tremark in tRemark)
      //      //{
      //      //  CPerson oPerson = new CPerson();
      //      //  TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == tremark.StrPersonId);
      //      //  MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
      //      //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //      //  {
      //      //    arlPerson.Add(oPerson);
      //      //  }
      //      //}

      //      //foreach (TObituary tobituary in db.TObituaries.Where(t => t.StrObituary.Contains(oFilterPersons.wildCardText)))
      //      //{
      //      //  CPerson oPerson = new CPerson();
      //      //  TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == tobituary.StrPersonId);
      //      //  MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
      //      //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //      //  {
      //      //    arlPerson.Add(oPerson);
      //      //  }
      //      //}

      //      //foreach (TPersonPortrait tpersonPortrait in db.TPersonPortraits.Where(t => t.StrRemarks.Contains(oFilterPersons.wildCardText) || t.StrTitle.Contains(oFilterPersons.wildCardText)))
      //      //{
      //      //  CPerson oPerson = new CPerson();
      //      //  TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == tpersonPortrait.StrPersonId);
      //      //  MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
      //      //  if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
      //      //  {
      //      //    arlPerson.Add(oPerson);
      //      //  }
      //      //}
      //    }

      //return arlPerson;
      //}
      //catch (Exception)
      //{
      //  throw;
      //}
    }


    //public async Task<IEnumerable<ValueObject.Person>> Migration(CancellationToken token)
    //{
    //  var persons = await _context.Persons.ToListAsync(token);
    //  var personGenderStatusess = await _context.PersonGenderStatuses.ToListAsync(token);

    //  var _personGenderStatusess = personGenderStatusess.ToList();

    //  var result = persons.Select(async p =>
    //  {
    //   // return await GetAllPersonAsync(p, token);
    //  });

    //  return (IEnumerable<ValueObject.Person>)result.ToList();
    //}

    public async Task<ValueObject.Person> ReadPersonRelationById(Guid Id, CancellationToken token)
    {
      var relation = await _context.PersonRelations.SingleOrDefaultAsync(x => x.Id == Id, token);
      
      if (relation != null)
      {
        ValueObject.Person person = relation.Person; // automatisch deserialisiert
        Console.WriteLine(person.FirstName);
        // Console.WriteLine($"Kinder: {string.Join(", ", person.Kinder.Select(k => k.Name))}");
        return person;
      }

      return null;
    }

    internal async Task<bool> AddAllPersonRelations(CancellationToken token)
    {
      //await _context.PersonRelations.ExecuteDeleteAsync(token);
      var persons = await _context.Persons.ToListAsync(token);
      foreach (var person in persons)
      {
        await AddPersonRelationByPerson(person.PersonRefId, token);
      }

      return true;
    }
    internal async Task<bool> AddPersonRelationByPerson(String Id, CancellationToken token)
    {
      try
      {
        var person = await _context.Persons.SingleOrDefaultAsync(x => x.PersonRefId == Id, token);
        if (person == null) return false;

        if (person.PersonRefId == "I4824")
        {
          person.PersonRefId = "I4824";
        }

        var personRel = await AddPersonRelations(person, token);
        var exist = await _context.PersonRelations.AnyAsync(x => x.Id == person.Id, token);
        if (exist)
        {
          _context.PersonRelations.Update(new Entity.PersonRelation
          {
            Id = person.Id,
            Person = personRel,
            Active = true,
            UpdateTimestamp = DateTime.Now
          });
        }
        else
        {
          _context.PersonRelations.Add(new Entity.PersonRelation
          {
            Id = person.Id,
            Person = personRel,
            Active = true,
            AddTimestamp = DateTime.Now
            //UpdateTimestamp = DateTime.Now
          });
        }
        _context.SaveChanges();

        var rel = await ReadPersonRelationById(person.Id, token);
        return true;
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
      }
      return false;
    }

    internal async Task<ValueObject.Person> AddPersonRelations(Entity.Person person, CancellationToken token)
    {
      try
      {
       

        if (person == null) return null;

        if(person.PersonRefId == "I4824")
        {
          person.PersonRefId = "I4824";
        }
        // 2. Basis-Person in ValueObject umwandeln
        var basePerson = new ValueObject.PersonRelation
        {
          Person = await GetPersonWithParentsAsync(person, token)
        };

        // 3. Partner hinzufügen (alle schon geladen)
        basePerson.Person.Partners = new List<ValueObject.Partner>();
        var partners = await _context.Partners.Where(x => x.PersonId == person.Id).ToListAsync(token);
        foreach (var p in partners)
        {
          var pd = await GetPartnerAsync(p.PartnerId, p.PersonId, token);
          basePerson.Person.AddPartner(pd);
        }

        // 4. Kinder hinzufügen (alle schon geladen)
        basePerson.Person.Childrens = new List<ValueObject.Person>();
        var parentChilds = await _context.ParentChilds.Where(x => x.Parent.Id == person.Id).ToListAsync(token);
        foreach (var c in parentChilds)
        {
          if (!basePerson.Person.Childrens.Any(x=> x.Id == c.Child.Id))
          {

            //basePerson.Person.AddChildren(MapPerson(c.Child,c.Parent));
           var child = await GetPersonWithParentChildsAsync(c, token);
            basePerson.Person.AddChildren(child);
          }
         
        }

        return basePerson.Person;
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
        return null;
      }
    }

    internal async Task<ValueObject.Person> AddPersonRelationsOri(Guid personId, CancellationToken token)
    {
      try
      {

        var basePerson = new ValueObject.PersonRelation();

        // 1. Basis-Person abrufen
        var person = await _context.Persons
            .SingleOrDefaultAsync(x => x.Id == personId, token);

        if (person == null) return null;

        // 2.Basis-Person hinzufügen
        basePerson.Person = await GetPersonWithParentsAsync(person, token);

        // 3. Partner-Beziehungen abrufen
        var partners = await _context.Partners.Where(x => x.PersonId == personId).ToListAsync(token);

        basePerson.Person.Partners = new List<ValueObject.Partner>();
        foreach (var p in partners)
        {
          var pd = await GetPartnerAsync(p.PartnerId, p.PersonId, token);
          basePerson.Person.AddPartner(pd);
        }

        // 4. Kinder abrufen
        var childrens = await _context.Persons.Where(x => x.FatherId == personId || x.MotherId == personId).ToListAsync(token);
        basePerson.Person.Childrens = new List<ValueObject.Person>();
        foreach (var c in childrens)
        {
          var child = await GetPersonWithParentsAsync(c, token);
          basePerson.Person.AddChildren(child);
        }

        return basePerson.Person;
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
        return null;
      }
    }


    //internal async Task<ValueObject.Person> ORIAddPersonRelations(Guid personId, CancellationToken token)
    //{
    //  try
    //  {
    //    // 1. Basis-Person abrufen
    //    var person = await _context.Persons
    //        .SingleOrDefaultAsync(x => x.Id == personId, token);

    //    if (person == null) return null;

    //    var personRel = new ValueObject.Person(personId);

    //    // Basis-Person hinzufügen
    //    personRel.BasePerson = await GetAllPersonAsync(person, token);




    //    // 4. Kinder abrufen
    //    var childrens = await _context.Persons
    //        .Where(x => x.FatherId == personId || x.MotherId == personId)
    //        .ToListAsync(token);

    //    // 5. ValueObject.Person erstellen


    //    // Partner-Personen hinzufügen (sequentiell)
    //    // 3. Partner-Personen abrufen
    //    var partnerPersons = await (from pr in _context.Partners
    //                                join pe in _context.Persons on pr.PartnerId equals pe.Id
    //                                where pr.PersonId == personId
    //                                select pe)
    //                        .ToListAsync(token);
    //    personRel.BasePerson.PersonPartners = new List<ValueObject.Person>();
    //    foreach (var p in partnerPersons)
    //    {
    //      var pp = await GetAllPersonAsync(p, token);
    //      personRel.BasePerson.AddPartnerPerson(pp);
    //    }

    //    // Partner-Details hinzufügen (sequentiell)
    //    // 2. Partner-Beziehungen abrufen
    //    var partners = await _context.Partners
    //        .Where(x => x.PersonId == personId)
    //        .ToListAsync(token);

    //    personRel.BasePerson.Partners = new List<ValueObject.Partner>();
    //    foreach (var p in partners)
    //    {
    //      var pd = await GetPartnerAsync(p.PartnerId, p.PersonId, token);
    //      personRel.BasePerson.AddPartner(pd);
    //    }

    //    // Kinder hinzufügen (sequentiell)
    //    personRel.BasePerson.Childrens = new List<ValueObject.Person>();
    //    foreach (var c in childrens)
    //    {
    //      var child = await GetAllPersonAsync(c, token);
    //      personRel.BasePerson.AddChildren(child);
    //    }

    //    return personRel;
    //  }
    //  catch (Exception ex)
    //  {
    //    Debug.WriteLine(ex);
    //    return null;
    //  }
    //}

    public async Task<IEnumerable<ValueObject.Person>> GetAllPersonsAsync(CancellationToken token)
    {
      var persons = await _context.Persons.ToListAsync(token);
      var personGenderStatusess = await _context.PersonGenderStatuses.ToListAsync(token);

      var _personGenderStatusess = personGenderStatusess.ToList();

      var result = persons.Select(async p =>
      {
        return await GetPersonWithParentsAsync(p, token);
      });

      return (IEnumerable<ValueObject.Person>)result.ToList();
    }

    //// <summary>
    ///// Ruft asynchron ein <see cref="ValueObject.Person"/>-Objekt mit detaillierten Informationen ab, 
    ///// einschließlich Vater und Mutter, falls vorhanden.
    ///// </summary>
    ///// <remarks>
    ///// Die Methode fragt die Datenbank ab, um den Vater und die Mutter der angegebenen Person zu laden, 
    ///// sofern deren jeweilige IDs vorhanden sind. Wenn Vater oder Mutter nicht gefunden werden, 
    ///// bleibt die entsprechende Eigenschaft im zurückgegebenen <see cref="ValueObject.Person"/>-Objekt <c>null</c>.
    ///// </remarks>
    ///// <param name="p">
    ///// Das ValueObject Person, das die Person repräsentiert, deren Details abgerufen werden sollen.
    ///// </param>
    ///// <param name="token">
    ///// Ein CancellationToken, der während des Wartens auf den Abschluss der Aufgabe beobachtet wird.
    ///// </param>
    ///// <returns>
    ///// Ein ValueObject.Person mit den Details der angegebenen Person, 
    ///// einschließlich Vater und Mutter als <see cref="ValueObject.Person"/>-Objekte, falls zutreffend.
    ///// </returns>
    ///// 
    internal async Task<ValueObject.Person> GetPersonWithParentsAsync(Entity.Person p, CancellationToken token)
    {
      try
      {
        var father = p.FatherId == Guid.Empty ? null : await _context.Persons.SingleOrDefaultAsync(x => x.Id == p.FatherId);
        var mother = p.MotherId == Guid.Empty ? null : await _context.Persons.SingleOrDefaultAsync(x => x.Id == p.MotherId);

        var voFather = father == null ? null : new ValueObject.Person(id: father.Id, personID: father.PersonRefId, familyName: father.FamilyName, firstName: father.FirstName,
           status: father.Status, birthPlace: father.BirthPlace, deathPlace: father.DeathPlace, burPlace: father.BurPlace,
           race: father.Race, work: father.Work, mameMerges: father.NameMerges, nickname: father.Nickname,
           birthDate: father.BirthDate, deathDate: father.DeathDate, burDate: father.BurDate,
           father: null, mother: null, father.Active);

        var voMother = mother == null ? null : new ValueObject.Person(id: mother.Id, personID: mother.PersonRefId, familyName: mother.FamilyName, firstName: mother.FirstName,
         status: mother.Status, birthPlace: mother.BirthPlace, deathPlace: mother.DeathPlace, burPlace: mother.BurPlace,
         race: mother.Race, work: mother.Work, mameMerges: mother.NameMerges, nickname: mother.Nickname,
         birthDate: mother.BirthDate, deathDate: mother.DeathDate, burDate: mother.BurDate,
         father: null, mother: null, mother.Active);

        return new ValueObject.Person(id: p.Id, personID: p.PersonRefId, familyName: p.FamilyName, firstName: p.FirstName,
          status: p.Status, birthPlace: p.BirthPlace, deathPlace: p.DeathPlace, burPlace: p.BurPlace,
          race: p.Race, work: p.Work, mameMerges: p.NameMerges, nickname: p.Nickname,
          birthDate: p.BirthDate, deathDate: p.DeathDate, burDate: p.BurDate,
          father: voFather, mother: voMother, p.Active);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
        return null;
      }
    }

    internal async Task<ValueObject.Person> GetPersonWithParentChildsAsync(Entity.ParentChild p, CancellationToken token)
    {
      try
      {
        var father = p.Parent.Id == Guid.Empty ? null : await _context.Persons.SingleOrDefaultAsync(x => x.Id == p.Parent.Id);
        var mother = p.Parent.Id == Guid.Empty ? null : await _context.Persons.SingleOrDefaultAsync(x => x.Id == p.Parent.Id);

        var voFather = father == null ? null : new ValueObject.Person(id: father.Id, personID: father.PersonRefId, familyName: father.FamilyName, firstName: father.FirstName,
           status: father.Status, birthPlace: father.BirthPlace, deathPlace: father.DeathPlace, burPlace: father.BurPlace,
           race: father.Race, work: father.Work, mameMerges: father.NameMerges, nickname: father.Nickname,
           birthDate: father.BirthDate, deathDate: father.DeathDate, burDate: father.BurDate,
           father: null, mother: null, father.Active);

        var voMother = mother == null ? null : new ValueObject.Person(id: mother.Id, personID: mother.PersonRefId, familyName: mother.FamilyName, firstName: mother.FirstName,
         status: mother.Status, birthPlace: mother.BirthPlace, deathPlace: mother.DeathPlace, burPlace: mother.BurPlace,
         race: mother.Race, work: mother.Work, mameMerges: mother.NameMerges, nickname: mother.Nickname,
         birthDate: mother.BirthDate, deathDate: mother.DeathDate, burDate: mother.BurDate,
         father: null, mother: null, mother.Active);

        
        return new ValueObject.Person(id: p.Child.Id, personID: p.Child.PersonRefId, familyName: p.Child.FamilyName, firstName: p.Child.FirstName,
          status: p.Child.Status, birthPlace: p.Child.BirthPlace, deathPlace: p.Child.DeathPlace, burPlace: p.Child.BurPlace,
          race: p.Child.Race, work: p.Child.Work, mameMerges: p.Child.NameMerges, nickname: p.Child.Nickname,
          birthDate: p.Child.BirthDate, deathDate: p.Child.DeathDate, burDate: p.Child.BurDate,
          father: voFather, mother: voMother, p.Active);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
        return null;
      }
    }


    internal async Task<ValueObject.Partner> GetPartnerAsync(Guid personId, Guid partnerId, CancellationToken token)
    {
      try
      {
        //var partner = await _context.Partners.SingleOrDefaultAsync(x => x.PersonId == partnerId && x.PartnerId== personId, token);

        //var person = await _context.Persons.SingleOrDefaultAsync(x => x.Id == personId, token);

        //if (partner == null || person == null)
        //  return null;

        //var pa = new ValueObject.Partner(partnerId: partner.PartnerId, personId: person.Id)
        //{
        //  MarriageDateTime = partner.MarriageDateTime,
        //  DivorceDateTime = partner.DivorceDateTime,
        //  PartnerPerson = await GetPersonWithParentsAsync(
        //        await _context.Persons.SingleOrDefaultAsync(x => x.Id == partner.PartnerId, token),
        //        token
        //    ),
        //  Person = await GetPersonWithParentsAsync(person, token),
        //  Status = partner.Status,
        //  Active = partner.Active
        //};

        //return pa;

        var partner = await _context.Partners.SingleOrDefaultAsync(x => x.PersonId == partnerId && x.PartnerId == personId, token);
        var person = await _context.Persons.SingleOrDefaultAsync(x => x.Id == personId, token);
        var personPartner = await _context.Persons.SingleOrDefaultAsync(x => x.Id == personId, token);


        if (partner == null || person == null || personPartner == null)
          return null;

        var pa = new ValueObject.Partner(partnerId: partner.PartnerId, personId: person.Id);
        pa.MarriageDateTime = partner.MarriageDateTime;
        pa.DivorceDateTime = partner.DivorceDateTime;
        pa.PartnerPerson = await GetPersonWithParentsAsync(person, token);
        pa.Person = await GetPersonWithParentsAsync(person, token);
        pa.ConnectionRole = partner.ConnectionRole;
        pa.Active = partner.Active;

        return pa;
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
        return null;
      }
    }

    protected async Task<ValueObject.Person> GetPersonAsync(Entity.Person person)
    {
      var valueObjectFather = null as ValueObject.Person;
      var valueObjectMother = null as ValueObject.Person;
      if (person == null)
      {
        return null;
      }
      if (person.FatherId != null && person.FatherId != Guid.Empty)
      {
        var father = await _context.Persons.SingleOrDefaultAsync(x => x.Id == person.FatherId);

        valueObjectFather = new ValueObject.Person(
          id: father.Id,
          personID: father.PersonRefId,
          familyName: father.FamilyName,
          firstName: father.FirstName,
          status: father.Status,
          birthPlace: father.BirthPlace,
          deathPlace: father.DeathPlace,
          burPlace: father.BurPlace,
          race: father.Race,
          work: father.Work,
          mameMerges: father.NameMerges,
          nickname: father.Nickname,
          birthDate: father.BirthDate,
          deathDate: father.DeathDate,
          burDate: father.BurDate,
          father: null,
          mother: null,
          father.Active);
      }

      if (person.MotherId != null && person.MotherId != Guid.Empty)
      {
        var mother = await _context.Persons.SingleOrDefaultAsync(x => x.Id == person.MotherId);

        valueObjectFather = new ValueObject.Person(id: mother.Id,
          personID: mother.PersonRefId,
          familyName: mother.FamilyName,
          firstName: mother.FirstName,
          status: mother.Status,
          birthPlace: mother.BirthPlace,
          deathPlace: mother.DeathPlace,
          burPlace: mother.BurPlace,
          race: mother.Race,
          work: mother.Work,
          mameMerges: mother.NameMerges,
          nickname: mother.Nickname,
          birthDate: mother.BirthDate,
          deathDate: mother.DeathDate,
          burDate: mother.BurDate,
          father: null,
          mother: null,
          mother.Active);
      }

      return new ValueObject.Person(id: person.Id, personID: person.PersonRefId, familyName: person.FamilyName, firstName: person.FirstName,
          status: person.Status, birthPlace: person.BirthPlace, deathPlace: person.DeathPlace, burPlace: person.BurPlace,
          race: person.Race, work: person.Work, mameMerges: person.NameMerges, nickname: person.Nickname,
          birthDate: person.BirthDate, deathDate: person.DeathDate, burDate: person.BurDate,
          father: valueObjectFather, mother: valueObjectMother, person.Active);
    }
    private ValueObject.Person MapPerson(Entity.Person p, ValueObject.Person father, ValueObject.Person mother)
    {
      var obj = new ValueObject.Person(
        id: p.Id,
        personID: p.PersonRefId,
        familyName: p.FamilyName,
        firstName: p.FirstName,
        status: p.Status,
        birthPlace: p.BirthPlace,
        deathPlace: p.DeathPlace,
        burPlace: p.BurPlace,
        race: p.Race,
        work: p.Work,
        mameMerges: p.NameMerges,
        nickname: p.Nickname,
        birthDate: p.BurDate,
        deathDate: p.DeathDate,
        burDate: p.BurDate,
        father: father,
        mother: mother,
        active: p.Active
        );
      return obj;
    }


  }
}
