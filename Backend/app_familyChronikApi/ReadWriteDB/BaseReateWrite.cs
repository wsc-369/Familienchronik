namespace app_familyChronikApi.ReadWriteDB
{
  public abstract class BaseReateWrite
  {

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

    //protected async Task<Person> GetPersonAsync(appAhnenforschungData.DataModel.Person person, MyDatabaseContext _context, CancellationToken token)
    //{
    //  var valueObjectFather = null as Person;
    //  var valueObjectMother = null as Person;
    //  if (person == null)
    //  {
    //    return null;
    //  }
    //  if (person.FatherId != null && person.FatherId != Guid.Empty)
    //  {
    //    var father = await _context.Persons.SingleOrDefaultAsync(x => x.Id == person.FatherId, token);

    //    valueObjectFather = new Person(id: father.Id,
    //      personID: father.PersonRefId,
    //      familyname: father.Familyname,
    //      firstName: father.FirstName,
    //      status: father.Status,
    //      birthPlace: father.BirthPlace,
    //      deathPlace: father.DeathPlace,
    //      burPlace: father.BurPlace,
    //      race: father.Race,
    //      work: father.Work,
    //      mameMerges: father.NameMerges,
    //      nickname: father.Nickname,
    //      birthDate: father.BirthDate,
    //      deathDate: father.DeathDate,
    //      burDate: father.BurDate,
    //      father: null,
    //      mother: null,
    //      father.Active);
    //  }

    //  if (person.MotherId != null && person.MotherId != Guid.Empty)
    //  {
    //    var mother = await _context.Persons.SingleOrDefaultAsync(x => x.Id == person.MotherId, token);

    //    valueObjectFather = new Person(id: mother.Id,
    //      personID: mother.PersonRefId,
    //      familyname: mother.Familyname,
    //      firstName: mother.FirstName,
    //      status: mother.Status,
    //      birthPlace: mother.BirthPlace,
    //      deathPlace: mother.DeathPlace,
    //      burPlace: mother.BurPlace,
    //      race: mother.Race,
    //      work: mother.Work,
    //      mameMerges: mother.NameMerges,
    //      nickname: mother.Nickname,
    //      birthDate: mother.BirthDate,
    //      deathDate: mother.DeathDate,
    //      burDate: mother.BurDate,
    //      father: null,
    //      mother: null,
    //      mother.Active);
    //  }

    //  return new Person(id: person.Id, personID: person.PersonRefId, familyname: person.Familyname, firstName: person.FirstName,
    //      status: person.Status, birthPlace: person.BirthPlace, deathPlace: person.DeathPlace, burPlace: person.BurPlace,
    //      race: person.Race, work: person.Work, mameMerges: person.NameMerges, nickname: person.Nickname,
    //      birthDate: person.BirthDate, deathDate: person.DeathDate, burDate: person.BurDate,
    //      father: valueObjectFather, mother: valueObjectMother, person.Active);
    //}


  }
}
