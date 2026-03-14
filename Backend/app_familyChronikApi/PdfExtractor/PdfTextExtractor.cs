namespace app_familyBackend.PdfExtractor
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using UglyToad.PdfPig;

  public class PdfTextExtractor
  {
    // Erweiterte Encoding-Ersetzungen für häufige PDF-Kodierungsfehler
    private static readonly Dictionary<string, string> EncodingReplacements = new Dictionary<string, string>
    {
        // Anführungszeichen und Apostrophe
        {"\u00E2\u0080\u0098", "'"}, {"\u00E2\u0080\u0099", "'"},
        {"\u00E2\u0080\u009C", "\""}, {"\u00E2\u0080\u009D", "\""},
        {"\u00E2\u0080\u0093", "-"}, {"\u00E2\u0080\u0094", "-"},
        {"\u00E2\u0080\u00A6", "..."},
        
        // ISO-8859-1/Windows-1252 zu UTF-8 Fehler für deutsche Umlaute
        {"Ã¤", "ä"}, {"Ã¶", "ö"}, {"Ã¼", "ü"},
        {"Ã„", "Ä"}, {"Ã–", "Ö"}, {"Ãœ", "Ü"}, 
        {"ÃŸ", "ß"},
        
        // Doppelt kodierte Umlaute
        {"Ã\u0083Â¤", "ä"}, {"Ã\u0083Â¶", "ö"}, {"Ã\u0083Â¼", "ü"},
        {"Ã\u0083\u0084", "Ä"}, {"Ã\u0083\u0096", "Ö"}, {"Ã\u0083\u009C", "Ü"},
        
        // MacRoman Encoding Fehler
        {"‰", "ä"}, {"˜", "ü"}, {"˚", "ö"},
        
        // HTML-Entities
        {"&auml;", "ä"}, {"&ouml;", "ö"}, {"&uuml;", "ü"},
        {"&Auml;", "Ä"}, {"&Ouml;", "Ö"}, {"&Uuml;", "Ü"},
        {"&szlig;", "ß"},
        
        // PDF-spezifische Kodierungsfehler
        {"Â", ""}, {"â€™", "'"}, {"â€œ", "\""}, {"â€", "\""},
    };

    // Fehlerhafte Wörter wo "i" als "ä" erkannt wurde (muss VOR ContextualWordReplacements!)
    private static readonly Dictionary<string, string> FixBrokenItoAe = new Dictionary<string, string>
    {
        // Verein-Varianten
        {"Veräen", "Verein"}, {"veräen", "verein"}, {"Vereäns", "Vereins"}, {"vereäns", "vereins"},
        {"Vereänsausschluss", "Vereinsausschluss"}, {"vereänsausschluss", "vereinsausschluss"},
        {"Vereänskassäer", "Vereinskassier"}, {"vereänskassäer", "vereinskassier"},
        
        // Mitglied-Varianten
        {"Mätgläed", "Mitglied"}, {"mätgläed", "mitglied"},
        {"Mätgläeder", "Mitglieder"}, {"mätgläeder", "mitglieder"},
        {"Mätgläederbeätrag", "Mitgliederbeitrag"}, {"mätgläederbeätrag", "mitgliederbeitrag"},
        
        // Artikel
        {"Artäkel", "Artikel"}, {"artäkel", "artikel"},
        
        // Hiermit
        {"häermät", "hiermit"}, {"Häermät", "Hiermit"},
        
        // Bei
        {"beä", "bei"}, {"Beä", "Bei"},
        
        // Schriftlich
        {"schräftlächen", "schriftlichen"}, {"Schräftlächen", "Schriftlichen"},
        {"schräftläch", "schriftlich"}, {"Schräftläch", "Schriftlich"},
        
        // Papier
        {"Päpäerform", "Papierform"}, {"papäerform", "papierform"},
        {"papäerenen", "papierenen"}, {"Päpäerenen", "Papierenen"},
        {"Päpäer", "Papier"}, {"papäer", "papier"},
        
        // Diesem/Dieser
        {"däesem", "diesem"}, {"Däesem", "Diesem"},
        {"däeser", "dieser"}, {"Däeser", "Dieser"},
        {"däeses", "dieses"}, {"Däeses", "Dieses"},
        
        // Bitte
        {"bätte", "bitte"}, {"Bätte", "Bitte"},
        
        // Die
        {"däe", "die"}, {"Däe", "Die"},
        
        // Verpflichte
        {"verpflächte", "verpflichte"}, {"Verpflächte", "Verpflichte"},
        
        // Mich
        {"mäch", "mich"}, {"Mäch", "Mich"},
        
        // Eigen
        {"eägenen", "eigenen"}, {"Eägenen", "Eigenen"},
        {"eägene", "eigene"}, {"Eägene", "Eigene"},
        {"eägener", "eigener"}, {"Eägener", "Eigener"},
        
        // Weise/Weisen
        {"weäse", "weise"}, {"Weäse", "Weise"},
        {"weäsen", "weisen"}, {"Weäsen", "Weisen"},
        
        // Hin
        {"hän", "hin"}, {"Hän", "Hin"},
        {"hänaus", "hinaus"}, {"Hänaus", "Hinaus"},
        
        // Sie
        {"säe", "sie"}, {"Säe", "Sie"},
        
        // Ausdrücklich
        {"ausdrückläch", "ausdrücklich"}, {"Ausdrückläch", "Ausdrücklich"},
        
        // Ist
        {"äst", "ist"}, {"Äst", "Ist"},
        
        // Systematisch
        {"süstematäsch", "systematisch"}, {"Süstematäsch", "Systematisch"},
        
        // Weiter
        {"weäterzuverwenden", "weiterzuverwenden"}, {"Weäterzuverwenden", "Weiterzuverwenden"},
        {"weäterzugeben", "weiterzugeben"}, {"Weäterzugeben", "Weiterzugeben"},
        {"weäter", "weiter"}, {"Weäter", "Weiter"},
        
        // Mir
        {"mär", "mir"}, {"Mär", "Mir"},
        
        // Im
        {"äm", "im"}, {"Äm", "Im"},
        
        // Datenschutzrechtliche
        {"datenschutzrechtläche", "datenschutzrechtliche"}, {"Datenschutzrechtläche", "Datenschutzrechtliche"},
        
        // Wird
        {"wärd", "wird"}, {"Wärd", "Wird"},
        
        // Derzeit
        {"derzeät", "derzeit"}, {"Derzeät", "Derzeit"},
        
        // Eingang
        {"Eängang", "Eingang"}, {"eängang", "eingang"},
        
        // Familienchronik
        {"Famäläenchronäk", "Familienchronik"}, {"famäläenchronäk", "familienchronik"},
        {"Famäläen", "Familien"}, {"famäläen", "familien"},
        
        // Beitritt
        {"Beäträtt", "Beitritt"}, {"beäträtt", "beitritt"},
        {"Beätrag", "Beitrag"}, {"beätrag", "beitrag"},
        
        // Triesenberg
        {"Träesenberg", "Triesenberg"}, {"träesenberg", "triesenberg"},
        
        // E-Mail
        {"E-Maäl", "E-Mail"}, {"e-maäl", "e-mail"},
        {"Maäl", "Mail"}, {"maäl", "mail"},
        
        // Lege ich in
        {"lege äch än", "lege ich in"},
        
        // Kenntnis
        {"Kenntnäs", "Kenntnis"}, {"kenntnäs", "kenntnis"},
        
        // Dritte
        {"Drätte", "Dritte"}, {"drätte", "dritte"},
        
        // In/an/mit (häufige Präpositionen)
        {"än", "in"}, {"Än", "In"},
        {"mät", "mit"}, {"Mät", "Mit"},
        
        // Ich
        {"äch", "ich"}, {"Äch", "Ich"},
    };

    // Kontextbasierte Umlaut-Korrekturen für ae/oe/ue Schreibweisen
    private static readonly Dictionary<string, string> ContextualWordReplacements = new Dictionary<string, string>
    {
        {"fuer", "für"}, {"Fuer", "Für"},
        {"ueber", "über"}, {"Ueber", "Über"},
        {"Muenchen", "München"}, {"muenchen", "münchen"},
        {"Zuerich", "Zürich"}, {"zuerich", "zürich"},
        {"Oesterreich", "Österreich"}, {"oesterreich", "österreich"},
        {"koennen", "können"}, {"Koennen", "Können"},
        {"moechte", "möchte"}, {"Moechte", "Möchte"},
        {"mussen", "müssen"}, {"Mussen", "Müssen"},
        {"wuerde", "würde"}, {"Wuerde", "Würde"},
        {"natuerlich", "natürlich"}, {"Natuerlich", "Natürlich"},
        {"gruessen", "grüßen"}, {"Gruessen", "Grüßen"},
        {"schoen", "schön"}, {"Schoen", "Schön"},
        {"Maerz", "März"}, {"maerz", "märz"},
        {"Jaeger", "Jäger"}, {"jaeger", "jäger"},
        {"Boerse", "Börse"}, {"boerse", "börse"},
        {"hoeren", "hören"}, {"Hoeren", "Hören"},
        {"gehoert", "gehört"}, {"Gehoert", "Gehört"},
    };

    public static string ExtractTextFromPdf(string filePath)
    {
      try
      {
        var textBuilder = new StringBuilder();
        using (PdfDocument document = PdfDocument.Open(filePath))
        {
          foreach (var page in document.GetPages())
          {
            var letters = page.Letters;
            if (letters == null || !letters.Any())
            {
              Console.WriteLine($"Seite {page.Number}: Kein Text → Scan oder OCR nötig");
            }
            else
            {
              Console.WriteLine($"Seite {page.Number}: Enthält echten Text");
            }

            foreach (var word in page.GetWords())
            {
              textBuilder.Append(word.Text);
              textBuilder.Append(" ");
            }
          }
        }
        return NormalizeText(textBuilder.ToString());
      }
      catch (Exception ex)
      {
        Console.WriteLine($"PDF-Extraktion fehlgeschlagen: {ex.Message}");
        return string.Empty;
      }
    }

    public static string ExtractText(string text)
    {
      try
      {
        return NormalizeText(text);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Extract Text fehlgeschlagen: {ex.Message}");
        return string.Empty;
      }
    }

    private static string NormalizeText(string text)
    {
      if (string.IsNullOrEmpty(text))
        return text;

      // 1. Worttrennung zuerst behandeln
      text = ResolveHyphenation(text);

      // 2. Kodierungsfehler korrigieren (längere Sequenzen zuerst)
      foreach (var kvp in EncodingReplacements.OrderByDescending(x => x.Key.Length))
      {
        text = text.Replace(kvp.Key, kvp.Value);
      }

      // 3. Unicode normalisieren (NFC - Canonical Composition)
      text = text.Normalize(NormalizationForm.FormC);

      // 4. Unicode-Sonderzeichen durch ASCII-Äquivalente ersetzen
      text = text.Replace("\u2018", "'").Replace("\u2019", "'")
                 .Replace("\u201C", "\"").Replace("\u201D", "\"")
                 .Replace("\u2013", "-").Replace("\u2014", "-")
                 .Replace("\u2026", "...")
                 .Replace("\u00A0", " ")
                 .Replace("\u200B", "")
                 .Replace("\u00AD", "")
                 .Replace("\u2007", " ")
                 .Replace("\u202F", " ");

      // 5. PDF-spezifische Störzeichen entfernen
      text = text.Replace("§", " ")
                 .Replace("¦", " ");

      // 6. WICHTIG: Fehlerhafte "i→ä" Ersetzungen korrigieren (längste Wörter zuerst!)
      foreach (var kvp in FixBrokenItoAe.OrderByDescending(x => x.Key.Length))
      {
        text = Regex.Replace(text, Regex.Escape(kvp.Key), kvp.Value, RegexOptions.IgnoreCase);
      }

      // 7. Kontextbasierte ae/oe/ue Wortkorrekturen (nur ganze Wörter)
      foreach (var kvp in ContextualWordReplacements)
      {
        text = Regex.Replace(text, $@"\b{Regex.Escape(kvp.Key)}\b", kvp.Value);
      }

      // 8. OCR-Fehler mit Kontext korrigieren
      text = CorrectOcrErrorsWithContext(text);

      // 9. Steuerzeichen entfernen
      text = Regex.Replace(text, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

      // 10. Whitespace normalisieren
      text = Regex.Replace(text, @"[ \t]+", " ");
      text = Regex.Replace(text, @"\r\n|\r|\n", "\n");

      // 11. PDF-Textstruktur wiederherstellen
      text = ReconstructNormalPdfText(text);

      return text.Trim();
    }

    /// <summary>
    /// Löst Worttrennung (Silbentrennung) auf
    /// </summary>
    private static string ResolveHyphenation(string text)
    {
      if (string.IsNullOrEmpty(text))
        return text;

      // Bindestrich + Zeilenumbruch = Worttrennung
      text = Regex.Replace(text, @"-\s*[\r\n]+\s*", "");
      
      // Bindestrich + Leerzeichen + Zeilenumbruch
      text = Regex.Replace(text, @"-\s+[\r\n]+\s*", "");

      // Soft hyphen entfernen
      text = text.Replace("\u00AD", "");

      // Non-breaking hyphen
      text = Regex.Replace(text, @"\u2011\s*[\r\n]+\s*", "");

      // Wörter ohne Bindestrich getrennt (min. 2 Zeichen pro Teil)
      text = Regex.Replace(text, @"([a-zäöüß]{2,})[\r\n]+([a-zäöüß]{2,})", "$1$2");

      return text;
    }

    /// <summary>
    /// Korrigiert OCR-Fehler nur in eindeutigen Kontexten
    /// </summary>
    private static string CorrectOcrErrorsWithContext(string text)
    {
      var ocrWordPatterns = new Dictionary<string, string>
      {
        // Städtenamen
        {@"\bMunchen\b", "München"},
        {@"\bZurich\b", "Zürich"},
        {@"\bKoln\b", "Köln"},
        {@"\bDusseldorf\b", "Düsseldorf"},
        {@"\bNurnberg\b", "Nürnberg"},
        
        // Häufige Nachnamen
        {@"\bMuller\b", "Müller"},
        {@"\bmuller\b", "müller"},
        
        // Häufige Verben
        {@"\bfuhren\b", "führen"},
        {@"\bFuhren\b", "Führen"},
        {@"\bmusse\b", "müsse"},
        
        // Grußformeln
        {@"\bgrusse\b", "grüße"},
        {@"\bGrusse\b", "Grüße"},
        {@"\bgrussen\b", "grüßen"},
        
        // Sonstige
        {@"\bschlussel\b", "schlüssel"},
        {@"\bSchlussel\b", "Schlüssel"},
        {@"\bturen\b", "türen"},
        {@"\bbruder\b", "brüder"},
      };

      foreach (var pattern in ocrWordPatterns)
      {
        text = Regex.Replace(text, pattern.Key, pattern.Value);
      }

      return text;
    }

    public static string ExtractKeywords(string text, int maxKeywords = 15)
    {
      if (string.IsNullOrEmpty(text)) return string.Empty;

      var words = TokenizeText(text);
      var filteredWords = FilterWords(words);
      var keywordFrequency = GetKeywordFrequency(filteredWords);
      var topKeywords = keywordFrequency
          .OrderByDescending(kvp => kvp.Value)
          .Take(maxKeywords)
          .Select(kvp => kvp.Key)
          .ToList();

      return string.Join(",", topKeywords);
    }

    private static List<string> TokenizeText(string text)
    {
      string cleaned = Regex.Replace(text, @"[^\wäöüÄÖÜßàáâãèéêëìíîïòóôõùúûü\-]", " ");
      return cleaned.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => w.Trim())
                    .Where(w => !string.IsNullOrEmpty(w))
                    .ToList();
    }

    public static List<string> FilterWords(List<string> words)
    {
      var stopWords = new HashSet<string>
      {
        "der", "die", "das", "und", "oder", "aber", "nicht", "ist", "sind", "war",
        "waren", "sein", "haben", "hat", "hatte", "hatten", "ich", "du", "er", "sie",
        "es", "wir", "ihr", "den", "dem", "des", "in", "auf", "mit", "nach", "von",
        "an", "bei", "zu", "als", "am", "um", "aus", "über", "durch", "gegen",
        "seit", "ohne", "gegenüber", "für", "bis", "zwischen", "neben", "auch",
        "eine", "ein", "einer", "einem", "eines", "wird", "werden", "wurde", "wurden",
        "kann", "können", "konnte", "konnten", "muss", "müssen", "musste", "mussten",
        "soll", "sollen", "sollte", "sollten", "darf", "dürfen", "durfte", "durften",
        "wenn", "dann", "weil", "dass", "ob", "wie", "was", "wer", "wo", "wann",
        "nur", "noch", "schon", "immer", "nie", "mehr", "weniger", "sehr", "viel",
        "mir", "mich", "ihm", "ihn", "sich", "uns", "euch", "ihnen", "im", "ins"
      };

      return words
          .Where(w => w.Length >= 3)
          .Where(w => !stopWords.Contains(w.ToLower()))
          .Where(w => !IsOnlyNumbers(w))
          .Select(w => w.ToLowerInvariant().Trim())
          .Where(w => !string.IsNullOrEmpty(w))
          .Distinct()
          .ToList();
    }

    private static bool IsOnlyNumbers(string word)
    {
      return Regex.IsMatch(word, @"^[\d\.,\-]+$");
    }

    private static Dictionary<string, int> GetKeywordFrequency(List<string> words)
    {
      return words.GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());
    }

    public static string GenerateSummary(string text, int maxLength = 200)
    {
      if (string.IsNullOrEmpty(text)) return string.Empty;

      var sentences = Regex.Split(text, @"(?<=[.!?])\s+")
                           .Where(s => !string.IsNullOrWhiteSpace(s))
                           .ToArray();
      
      if (sentences.Length == 0)
        return text.Substring(0, Math.Min(maxLength, text.Length)) + "...";

      var summary = new StringBuilder();
      foreach (var sentence in sentences.Take(3))
      {
        var trimmedSentence = sentence.Trim();
        if (summary.Length + trimmedSentence.Length > maxLength)
          break;
        
        summary.Append(trimmedSentence);
        if (!trimmedSentence.EndsWith(".") && !trimmedSentence.EndsWith("!") && !trimmedSentence.EndsWith("?"))
        {
          summary.Append(".");
        }
        summary.Append(" ");
      }

      var result = summary.ToString().Trim();
      return !string.IsNullOrEmpty(result)
        ? result + (result.EndsWith(".") || result.EndsWith("!") || result.EndsWith("?") ? ".." : "...")
        : text.Substring(0, Math.Min(maxLength, text.Length)) + "...";
    }

    private static string ReconstructNormalPdfText(string text)
    {
      if (string.IsNullOrEmpty(text))
        return text;

      // Weitere Worttrennung behandeln
      text = Regex.Replace(text, @"-\n(?=[a-zäöüß])", "");
      
      // Wörter ohne Bindestrich getrennt
      text = Regex.Replace(text, @"([a-zäöüß]{2,})\n(?=[a-zäöüß]{2,})", "$1");

      // Zeilenumbrüche entfernen wenn kein Satzende
      text = Regex.Replace(text, @"(?<![.!?:])\n(?!\n)", " ");

      // Doppelte Zeilenumbrüche als Absätze
      text = Regex.Replace(text, @"\n{2,}", "\n\n");

      // ✅ WICHTIG: Nach Satzzeichen IMMER ein Leerzeichen einfügen (falls fehlt)
      // Punkt/Ausrufe-/Fragezeichen direkt gefolgt von Großbuchstaben → Leerzeichen einfügen
      text = Regex.Replace(text, @"([.!?])([A-ZÄÖÜ])", "$1 $2");
      
      // Auch nach Komma/Doppelpunkt/Semikolon falls direkt Buchstabe folgt
      text = Regex.Replace(text, @"([,:;])([a-zA-ZäöüÄÖÜß])", "$1 $2");

      // Satzenden erkennen: Punkt + Leerzeichen + Großbuchstabe = neuer Absatz
      text = Regex.Replace(text, @"\.\s+([A-ZÄÖÜ])", ".\n\n$1");

      // Überschriften erkennen (kurze Zeilen ohne Satzzeichen am Ende)
      text = Regex.Replace(text, @"(?m)^([A-ZÄÖÜ][^\n.!?]{2,59})$", "\n\n$1\n\n");

      // Mehrfach-Leerzeichen reduzieren (aber NACH Satzzeichen mindestens eines lassen!)
      text = Regex.Replace(text, @"(?<![.!?,:;])\s{2,}", " ");
      text = Regex.Replace(text, @"(?<=[.!?,:;])\s{2,}", " ");

      // Absatzformatierung bereinigen
      text = Regex.Replace(text, @"\n ", "\n");
      text = Regex.Replace(text, @" \n", "\n");

      // Mehr als 2 Zeilenumbrüche reduzieren
      text = Regex.Replace(text, @"\n{3,}", "\n\n");

      // Leerzeichen am Anfang/Ende von Zeilen entfernen
      text = Regex.Replace(text, @"(?m)^\s+|\s+$", "");

      return text.Trim();
    }
  }
}