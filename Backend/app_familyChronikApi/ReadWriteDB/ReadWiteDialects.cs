using app_familyBackend.DataContext;
using ValueObject;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;


namespace app_familyChronikApi.ReadWriteDB
{
  public class ReadWiteDialects(MyDatabaseContext context)
  {
    private readonly MyDatabaseContext _context = context;


    public async Task<IEnumerable<ValueObject.DialectWord>> GetDialectWords(bool onlyActive, CancellationToken token)
    {
      try
      {
        List<ValueObject.DialectWord> arlDialectWord = new List<DialectWord>();
        var dialects = await _context.DialectWordCollection.ToListAsync(token);
        foreach (var dialect in dialects.Where(x => x.Active == onlyActive ? true : false))
        {
          arlDialectWord.Add(ToDialectWords(dialect));
        }
        return arlDialectWord;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public async Task<ValueObject.DialectWord> GetDialectWord(Guid id, CancellationToken token)
    {
      try
      {
        List<ValueObject.DialectWord> arlDialectWord = new List<DialectWord>();
        var dialect = await _context.DialectWordCollection.Where(x => x.Id == id).SingleOrDefaultAsync(token);

        if (dialect == null) return null;
        arlDialectWord.Add(ToDialectWords(dialect));

        return arlDialectWord.Single(x => x.Id == dialect.Id);
      }
      catch (Exception)
      {
        throw;
      }
    }

    public async Task<ValueObject.DialectWord> GetEmptyDialectWord()
    {
      try
      {
        return new DialectWord
        {
          Id = Guid.NewGuid(),
          Title = string.Empty,
          Description = string.Empty,
          Voice = string.Empty,
          PersonId = Guid.Empty,
          Active = false,
          PersonFamilyName = string.Empty,
          PersonFirstname = string.Empty
        };
      }
      catch (Exception)
      {
        throw;
      }
    }

    public async Task<IEnumerable<ValueObject.DialectWord>> getDialectFilteredWords(string word, CancellationToken token)
    {
      try
      {
        List<ValueObject.DialectWord> arlDialectWord = new List<DialectWord>();

        var dialects = await _context.DialectWordCollection.Where(x => x.Title.Contains(word)).ToListAsync(token);

        foreach (var dialect in dialects)
        {
          arlDialectWord.Add( ToDialectWords(dialect));
        }
        return arlDialectWord;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public async Task<ValueObject.DialectWord> AddDialectWord(DialectWord dialectWord, CancellationToken token)
    {
      try
      {
        var dialect = await _context.DialectWordCollection.Where(x => x.Id == dialectWord.Id).SingleOrDefaultAsync(token);// .ToListAsync(token);
        if (dialect == null)
        {
          dialect = new Entity.DialectWord();
          dialect.Id = dialectWord.Id;
          dialect.Title = dialectWord.Title;
          dialect.Description = dialectWord.Description;
          dialect.Voice = dialectWord.Voice;
          dialect.PersonId = dialectWord.PersonId;
          dialect.Active = dialectWord.Active;
          dialect.AddTimestamp = DateTime.Now;
          dialect.UpdateTimestamp = DateTime.Now;
        }
        await _context.DialectWordCollection.AddAsync(dialect);
        await _context.SaveChangesAsync(token);

        return ToDialectWords(dialect);

      }
      catch (Exception)
      {
        throw;
      }
    }

    public async Task<ValueObject.DialectWord> UpdateDialectWord(DialectWord dialectWord, CancellationToken token)
    {
      try
      {
        var dialect = await _context.DialectWordCollection.SingleOrDefaultAsync(x => x.Id == dialectWord.Id, token);

        if (dialect == null) return null;

        dialect.Title = dialectWord.Title;
        dialect.Description = dialectWord.Description;
        dialect.Voice = dialectWord.Voice;
        dialect.PersonId = dialectWord.PersonId;
        dialect.Active = dialectWord.Active;
        //dialect.AddTimestamp = DateTime.Now;
        dialect.UpdateTimestamp = DateTime.Now;

        _context.DialectWordCollection.Update(dialect);

        await _context.SaveChangesAsync(token);

        var added = new DialectWord
        {
          Id = dialect.Id,
          Title = dialect.Title,
          Description = dialect.Description,
          Voice = dialect.Voice,
          PersonId = dialect.PersonId
        };

        MergeVoiceSpeaker(added);

        return added;
      }
      catch (Exception)
      {
        throw;
      }
    }

    private DialectWord ToDialectWords(Entity.DialectWord dialectWord)
    {
      dialectWord.Description = dialectWord.Description.Replace("class=\"internal-link\"", "");

      var dw = new DialectWord
      {
        Id = dialectWord.Id,
        Title = dialectWord.Title,
        Description = ConvertWithRegex(dialectWord.Description),
        Voice = dialectWord.Voice,
        PersonId = dialectWord.PersonId,
        Active = dialectWord.Active
      };
      MergeVoiceSpeaker(dw);
      return dw;
    }

    private void MergeVoiceSpeaker(DialectWord dialectWord)
    {
      if (dialectWord.PersonId == Guid.Empty) return;
      var person = _context.Persons.Where(p => p.Id == dialectWord.PersonId).FirstOrDefault();
      if (person != null)
      {
        dialectWord.PersonFirstname = person.FirstName;
        dialectWord.PersonFamilyName = person.FamilyName;
      }
    }

    private static string ConvertWithRegex(string html)
    {
      if (string.IsNullOrEmpty(html))
        return html;
      var pattern = @"href\s*=\s*""ProjektWoerterbuch\?strFilter=([^""&>]+)""";
      return Regex.Replace(html, pattern, new MatchEvaluator(match =>
      {
        string filter = WebUtility.HtmlEncode(match.Groups[1].Value);
        return $"href=\"#\" class=\"filter-link\" data-filter=\"{filter}\"";
      }), RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private static string ConvertWithRegex1(string html)
    {
      if (string.IsNullOrEmpty(html))
        return html;

      var pattern = @"href\s*=\s*""ProjektWoerterbuch\?strFilter=([^""&>]+)""";

      return Regex.Replace(
          html,
          pattern,
          match =>
          {
            string filter = WebUtility.HtmlEncode(match.Groups[1].Value);

            return
              $"href=\"#\" " +
              $"class=\"filter-link\" " +
              $"data-filter=\"{filter}\" " +
              $"(click)=\"setFilter('{filter}'); $event.preventDefault()\"";
          },
          RegexOptions.IgnoreCase | RegexOptions.Compiled
      );
    }


  }
}
