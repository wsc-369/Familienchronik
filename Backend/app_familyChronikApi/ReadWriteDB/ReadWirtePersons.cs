using app_familyBackend.DataContext;
using appAhnenforschungData.Models.App;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Text;

namespace app_familyChronikApi.ReadWriteDB
{
  public class ReadWirtePersons(MyDatabaseContext context)
  {
    private readonly MyDatabaseContext _context = context;

    public IEnumerable<string> GroupedFamilyNames()
    {
      List<string> arlNames = new List<string>();
      IEnumerable<Entity.Person> groups = _context.Persons.Select(data => data.FamilyName)
                                    .Distinct()
                                    .Select(name => new Entity.Person()
                                    {
                                      FamilyName = name
                                    });

      foreach (var group in groups.Where(x => x.FamilyName.Length > 0).OrderBy(x => x.FamilyName))
      {
        arlNames.Add(group.FamilyName);
      }

      return arlNames;
    }

    public IEnumerable<string> GroupedFirstNames()
    {
      List<string> arlNames = new List<string>();
      IEnumerable<Entity.Person> groups = _context.Persons.Select(data => data.FirstName)
                                    .Distinct()
                                    .Select(name => new Entity.Person()
                                    {
                                      FirstName = name
                                    });

      foreach (var group in groups.Where(x => x.FirstName.Length > 0).OrderBy(x => x.FirstName))
      {
        arlNames.Add(group.FirstName);
      }

      return arlNames;
    }

    public async Task<string> GetPersonRefId(Guid id, CancellationToken token)
    {
      try
      {
        var person = await _context.Persons.SingleOrDefaultAsync(x => x.Id == id, token);

        return person.PersonRefId;

      }
      catch (Exception)
      {
        throw;
      }
    }

    public async Task<ValueObject.Person> GetPersonByRefId(string refId, CancellationToken token)
    {
      try
      {
        var person = await _context.Persons.SingleOrDefaultAsync(x => x.PersonRefId == refId, token);
        if (person == null) return null;

        return await GetPersonAsync(person, token);

      }
      catch (Exception)
      {
        throw;
      }
    }

    public async Task<ValueObject.Person> GetPersonById(Guid id, CancellationToken token)
    {
      try
      {
        var person = await _context.Persons.SingleOrDefaultAsync(x => x.Id == id, token);
        if (person == null) return null;

        return await GetPersonAsync(person, token);

      }
      catch (Exception)
      {
        throw;
      }
    }

    public async Task<ValueObject.Person> UpdatePersonByRefId(Guid id, ValueObject.Person person, CancellationToken token)
    {
      try
      {
        var p = await _context.Persons.SingleOrDefaultAsync(x => x.Id == id, token);
        if (p == null) return null;

        p.PersonRefId = person.PersonID;
        p.FamilyName = person.FamilyName;
        p.FirstName = person.FirstName;
        p.Status = person.Sex == "M" ? Entity.GenderStatusOfPerson.GenderStatus.Male : Entity.GenderStatusOfPerson.GenderStatus.Female;

        p.FatherId = person.Father?.Id;
        p.MotherId = person.Mother?.Id;

        p.BirthPlace = person.BirthPlace;
        p.DeathPlace = person.DeathPlace;
        p.BurPlace = person.BurPlace;

        p.Race = person.Race;
        p.Work = person.Work;
        p.NameMerges = person.NameMerges;
        p.Nickname = person.Nickname;


        p.BirthDate = person.BirthDate;
        p.DeathDate = person.DeathDate;
        p.BurDate = person.BurDate;

        p.BirtYear = person.BirthDate != DateTime.MinValue ? person.BirthDate.Year : 0;
        p.DeathYear = person.DeathDate != DateTime.MinValue ? person.DeathDate.Year : 0;

        p.BirtMonth = person.BirthDate != DateTime.MinValue ? person.BirthDate.Month : 0;
        p.DeathMonth = person.DeathDate != DateTime.MinValue ? person.DeathDate.Month : 0;

        p.BirtDay = person.BirthDate != DateTime.MinValue ? person.BirthDate.Day : 0;
        p.DeathDay = person.DeathDate != DateTime.MinValue ? person.DeathDate.Day : 0;

        p.Active = person.Active;
        p.UpdateTimestamp = DateTime.Now;


        await _context.SaveChangesAsync(token);

        return await GetPersonAsync(p, token);

      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public Entity.Person GetFamilyPersonFromXml(Entity.Person oPerson)
    {
      try
      {

        Entity.Person operson = null;
        if (_context.Families.Any(t => t.PersonId == oPerson.Id))
        {
          Entity.Family family = _context.Families.Single(t => t.PersonId == oPerson.Id);
          operson = DeserializationXMLToPersonObject(DecompressString(family.Tree));
        }

        return operson;
      }
      catch (Exception)
      {
        throw;
      }
    }



    protected async Task<ValueObject.Person> GetPersonAsync(Entity.Person person, CancellationToken token)
    {
      var valueObjectFather = null as ValueObject.Person;
      var valueObjectMother = null as ValueObject.Person;
      if (person == null)
      {
        return null;
      }
      if (person.FatherId != null && person.FatherId != Guid.Empty)
      {
        var father = await _context.Persons.SingleOrDefaultAsync(x => x.Id == person.FatherId, token);

        valueObjectFather = new ValueObject.Person(
          id:father.Id, 
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
        var mother = await _context.Persons.SingleOrDefaultAsync(x => x.Id == person.MotherId, token);

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

    private string DecompressString(string compressedText)
    {
      var gZipBuffer = Convert.FromBase64String(compressedText);

      using var memoryStream = new MemoryStream();
      int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
      memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

      var buffer = new byte[dataLength];
      memoryStream.Position = 0;

      using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);

      int totalRead = 0;
      while (totalRead < buffer.Length)
      {
        int bytesRead = gZipStream.Read(buffer, totalRead, buffer.Length - totalRead);
        if (bytesRead == 0) break;
        totalRead += bytesRead;
      }

      return Encoding.UTF8.GetString(buffer);
    }


    /// <summary>
    /// Compresses the string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    private string CompressString(string text)
    {
      byte[] buffer = Encoding.UTF8.GetBytes(text);
      var memoryStream = new MemoryStream();
      using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
      {
        gZipStream.Write(buffer, 0, buffer.Length);
      }

      memoryStream.Position = 0;

      var compressedData = new byte[memoryStream.Length];
      memoryStream.Read(compressedData, 0, compressedData.Length);

      var gZipBuffer = new byte[compressedData.Length + 4];
      Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
      Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
      return Convert.ToBase64String(gZipBuffer);
    }


    /// <summary>
    ///  Get XML Serialization: Deserialization
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    private Entity.Person DeserializationXMLToPersonObject(string xml)
    {
      try
      {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Entity.Person>(xml);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Get XML Serialization: Serialization
    /// </summary>
    /// <param name="oPerson"></param>
    /// <returns></returns>
    private string SerializationPersonObjectToXMLGeneric(CPerson oPerson)
    {
      try
      {
        string P = CompressString(ObjectToXMLGeneric(oPerson));
        return P;
      }
      catch (Exception)
      {
        throw;
      }
    }  /// <summary>
       /// Convert CPerson Object to XML: (Serialization)
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="filter"></param>
       /// <returns></returns>
    private String ObjectToXMLGeneric(CPerson filter)
    {
      return Newtonsoft.Json.JsonConvert.SerializeObject(filter, Formatting.Indented);
    }


  }
}
