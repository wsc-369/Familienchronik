namespace app_familyBackend.PdfExtractor
{
  using System;
  using System.Collections.Generic;
  using System.Reflection.PortableExecutable;
  using System.Text;
  using System.Text.RegularExpressions;
  using UglyToad.PdfPig;

  public class PdfTextExtractor
  {
    private static readonly Dictionary<string, string> EncodingReplacements = new Dictionary<string, string>
    {
        {"\u00E2\u0080\u0098", "'"}, {"\u00E2\u0080\u0099", "'"},
        {"\u00E2\u0080\u009C", "\""}, {"\u00E2\u0080\u009D", "\""},
        {"\u00E2\u0080\u0093", "-"}, {"\u00E2\u0080\u0094", "-"},
        {"\u00E2\u0080\u00A6", "..."}, {"Ã¤", "ä"}, {"Ã¶", "ö"}, {"Ã¼", "ü"},
        {"Ã„", "Ä"}, {"Ã–", "Ö"}, {"Ãœ", "Ü"}, {"ÃŸ", "ß"}
    };


    public static string ExtractTextFromPdf(string filePath)
    {
      try
      {
        var textBuilder = new StringBuilder();
        using (PdfDocument document = PdfDocument.Open(filePath)) // PdfReader
        {
          foreach (var page in document.GetPages())
          {
           


            var letters = page.Letters;
            if (letters == null || !letters.Any()) { 
              Console.WriteLine($"Seite {page.Number}: Kein Text → Scan oder OCR nötig"); 
            } else { 
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
      // Kodierungsfehler korrigieren
      foreach (var kvp in EncodingReplacements)
        text = text.Replace(kvp.Key, kvp.Value);

      // Unicode normalisieren
      text = text.Normalize(NormalizationForm.FormC);
      text = text.Replace("\u2018", "'").Replace("\u2019", "'")
                 .Replace("\u201C", "\"").Replace("\u201D", "\"")
                 .Replace("\u2013", "-").Replace("\u2014", "-")
                 .Replace("\u2026", "...").Replace("\u00A0", " ");

      // OCR‑Sonderzeichen aus deinen PDFs korrigieren
      text = text .Replace("§", " ")
                  .Replace("¦", " ")
                  .Replace("~", "ü")
                  .Replace("y", "ü") // häufiges OCR‑Problem bei ü
                  .Replace("i", "ä"); // häufiges OCR‑Problem bei ä


      // Whitespace normalisieren
      text = Regex.Replace(text, @"\s+", " ");
      text = Regex.Replace(text, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

      text = ReconstructNormalPdfText(text);
      return text.Trim();
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
      string cleaned = Regex.Replace(text, @"[^\wäöüÄÖÜß\-]", " ");
      return cleaned.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static List<string> FilterWords(List<string> words)
    {
      var stopWords = new HashSet<string>
        {
            "der", "die", "das", "und", "oder", "aber", "nicht", "ist", "sind", "war",
            "waren", "sein", "haben", "hat", "hatte", "hatten", "ich", "du", "er", "sie",
            "es", "wir", "ihr", "den", "dem", "des", "in", "auf", "mit", "nach", "von",
            "an", "bei", "zu", "als", "am", "um", "aus", "über", "durch", "gegen",
            "seit", "ohne", "gegenüber", "für", "bis", "zwischen", "neben", "auch"
        };

      return words
          .Where(w => w.Length >= 3)
          .Where(w => !stopWords.Contains(w.ToLower()))
          .Select(w => w.ToLowerInvariant().Trim())
          .Where(w => !string.IsNullOrEmpty(w))
          .ToList();
    }

    private static Dictionary<string, int> GetKeywordFrequency(List<string> words)
    {
      return words.GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());
    }

    public static string GenerateSummary(string text)
    {
      if (string.IsNullOrEmpty(text)) return string.Empty;

      var sentences = text.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
      return sentences.Length > 0 ? sentences[0].Trim() + "...." : text.Substring(0, Math.Min(100, text.Length)) + "....";
    }

    private static string ReconstructNormalPdfText(string text)
    {
      // 1. Bindestriche am Zeilenende entfernen (Worttrennung)
      text = Regex.Replace(text, @"-\s*\n\s*", "");

      // 2. Zeilenumbrüche entfernen, wenn sie mitten im Satz stehen
      // Regel: Wenn davor kein Punkt/Fragezeichen/Ausrufezeichen steht → Satz geht weiter
      text = Regex.Replace(text, @"(?<![.!?])\n(?!\n)", " ");

      // 3. Doppelte Zeilenumbrüche als Absatz erkennen
      text = Regex.Replace(text, @"\n{2,}", "\n\n");

      // 4. Satzgrenzen anhand typischer Strukturen erkennen
      // Punkt + Leerzeichen + Großbuchstabe → Absatz
      text = Regex.Replace(text, @"\. ([A-ZÄÖÜ])", ".\n\n$1");

      // 5. Überschriften erkennen (kurze Zeilen ohne Punkt)
      text = Regex.Replace(text, @"(?m)^(?!.*[.!?]).{3,60}$", m => "\n\n" + m.Value.Trim() + "\n\n");

      // 6. Mehrfach-Leerzeichen entfernen
      text = Regex.Replace(text, @"\s+", " ").Trim();

      // 7. Absatzformatierung wiederherstellen
      text = Regex.Replace(text, @"\n ", "\n");

      return text.Trim();
    }



  }


  //public class PdfTextExtractorOriginal
  //{
  //  private static readonly Dictionary<string, string> SpecialCharacterMap = new Dictionary<string, string>
  //  {
  //      {"\u00A0", " "}, // Non-breaking space
  //      {"\u2013", "-"}, // En-dash
  //      {"\u2014", "-"}, // Em-dash
  //      {"\u2018", "'"}, // Left single quotation mark
  //      {"\u2019", "'"}, // Right single quotation mark
  //      {"\u201C", "\""}, // Left double quotation mark
  //      {"\u201D", "\""}, // Right double quotation mark
  //      {"\u2026", "..."}, // Horizontal ellipsis
  //      {"\u00E4", "ä"}, // ä
  //      {"\u00F6", "ö"}, // ö
  //      {"\u00FC", "ü"}, // ü
  //      {"\u00DF", "ß"}, // ß
  //      {"\u00C4", "Ä"}, // Ä
  //      {"\u00D6", "Ö"}, // Ö
  //      {"\u00DC", "Ü"}, // Ü
  //  };

  //  public string ExtractTextFromPdf(string filePath)
  //  {
  //    try
  //    {
  //      var textBuilder = new StringBuilder();

  //      using (var document = PdfDocument.Open(filePath)) 
  //      { 
  //        foreach (var page in document.GetPages()) 
  //        { Console.WriteLine(page.Text);
  //          // Extrahiere Text aus der Seite
  //          foreach (var word in page.GetWords())
  //          {
  //            textBuilder.Append(word.Text);
  //            textBuilder.Append(" ");
  //          }
  //        } 
  //      }

  //      //using (PdfDocument document = PdfReader.Open(filePath))
  //      //{
  //      //  foreach (var page in document.GetPages())
  //      //  {
  //      //    // Extrahiere Text aus der Seite
  //      //    foreach (var word in page.GetWords())
  //      //    {
  //      //      textBuilder.Append(word.Text);
  //      //      textBuilder.Append(" ");
  //      //    }
  //      //  }
  //      //}

  //      // Bereinige und formatiere den Text
  //      string rawText = textBuilder.ToString();
  //      // return ExtractKeywordsFromText(rawText);


  //      return CleanAndFormatText(rawText);
  //    }
  //    catch (Exception ex)
  //    {
  //      Console.WriteLine($"Fehler bei der Textextraktion: {ex.Message}");
  //      return string.Empty;
  //    }
  //  }

  //  private string CleanAndFormatText(string rawText)
  //  {
  //    if (string.IsNullOrEmpty(rawText))
  //      return rawText;

  //    // Ersetze spezielle Zeichen
  //    foreach (var mapping in SpecialCharacterMap)
  //    {
  //      rawText = rawText.Replace(mapping.Key, mapping.Value);
  //    }

  //    // Entferne doppelte Leerzeichen
  //    rawText = Regex.Replace(rawText, @"\s+", " ");

  //    // Entferne führende und nachfolgende Leerzeichen
  //    rawText = rawText.Trim();

  //    // Behandle spezielle Formatierungen
  //    rawText = Regex.Replace(rawText, @"\s*([.!?])\s*", "$1 ");
  //    rawText = Regex.Replace(rawText, @"([.!?])\1+", "$1");

  //    // Entferne problematische Steuerzeichen
  //    rawText = Regex.Replace(rawText, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

  //    return rawText;
  //  }



  //  /// <summary>
  //  /// Extrahiert Schlüsselwörter aus PDF-Text mit umfassender Fehlertoleranz
  //  /// </summary>
  //  public static string ExtractKeywordsFromText(string text,
  //      int maxKeywords = 15,
  //      int minWordLength = 3,
  //      bool excludeNumbers = true,
  //      string[] stopWords = null)
  //  {
  //    if (string.IsNullOrEmpty(text))
  //      return string.Empty;

  //    // 1. Textnormalisierung
  //    string normalizedText = NormalizeText(text);

  //    // 2. Texttokenisierung
  //    var words = TokenizeText(normalizedText);

  //    // 3. Filtern und Bereinigen
  //    var filteredWords = FilterWords(words, minWordLength, excludeNumbers, stopWords);

  //    // 4. Häufigkeitsermittlung
  //    var keywordFrequency = GetKeywordFrequency(filteredWords);

  //    // 5. Top-KKeywords auswählen
  //    var topKeywords = keywordFrequency
  //        .OrderByDescending(kvp => kvp.Value)
  //        .Take(maxKeywords)
  //        .Select(kvp => kvp.Key)
  //        .ToList();

  //    return string.Join(",", topKeywords);
  //  }

  //  /// <summary>
  //  /// Normalisiert Text durch mehrstufige Kodierungs-Korrektur
  //  /// </summary>
  //  private static string NormalizeText(string text)
  //  {
  //    // Kodierungsprobleme korrigieren
  //    text = FixEncodingIssues(text);

  //    // Unicode-Zeichen normalisieren
  //    text = NormalizeUnicode(text);

  //    // Whitespace normalisieren
  //    text = Regex.Replace(text, @"\s+", " ");

  //    return text.Trim();
  //  }

  //  /// <summary>
  //  /// Kodierungsprobleme korrigieren (z.B. falsche UTF-8-Codierung)
  //  /// </summary>
  //  private static string FixEncodingIssues(string text)
  //  {
  //    if (string.IsNullOrEmpty(text))
  //      return text;

  //    // Korrektur häufiger Kodierungsfehler für Umlaute und Sonderzeichen
  //    var replacements = new Dictionary<string, string>
  //      {
  //          // Windows-1252 zu UTF-8 Fehler
  //          {"\u00E2\u0080\u0098", "'"},  // falsch kodiertes Anführungszeichen
  //          {"\u00E2\u0080\u0099", "'"},  // falsch kodiertes Hochkomma
  //          {"\u00E2\u0080\u009C", "\""},  // falsch kodiertes Anführungszeichen
  //          {"\u00E2\u0080\u009D", "\""},  // falsch kodiertes Anführungszeichen
  //          {"\u00E2\u0080\u0093", "-"},  // falsch kodierter Gedankenstrich
  //          {"\u00E2\u0080\u0094", "-"},  // falsch kodierter Gedankenstrich
  //          {"\u00E2\u0080\u00A6", "..."}, // falsch kodierter Auslassungspunkt

  //          // Umlaute und ß
  //          {"Ã¤", "ä"}, {"Ã¶", "ö"}, {"Ã¼", "ü"},
  //          {"Ã„", "Ä"}, {"Ã–", "Ö"}, {"Ãœ", "Ü"},
  //          {"ÃŸ", "ß"},

  //          // Akzentzeichen
  //          {"Ã¡", "á"}, {"Ã©", "é"}, {"Ã­", "í"}, {"Ã³", "ó"}, {"Ãº", "ú"},
  //          {"Ã ", "à"}, {"Ã¨", "è"}, {"Ã¬", "ì"}, {"Ã²", "ò"}, {"Ã¹", "ù"},
  //          {"Ã¢", "â"}, {"Ãª", "ê"}, {"Ã®", "î"}, {"Ã´", "ô"}, {"Ã»", "û"},
  //      };

  //    foreach (var kvp in replacements)
  //    {
  //      text = text.Replace(kvp.Key, kvp.Value);
  //    }

  //    // Entferne unerwünschte Steuerzeichen
  //    text = Regex.Replace(text, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

  //    return text;
  //  }

  //  /// <summary>
  //  /// Unicode-Zeichen normalisieren (z.B. verschiedene Anführungszeichen)
  //  /// </summary>
  //  private static string NormalizeUnicode(string text)
  //  {
  //    // Normalform C (NFC) verwenden für konsistente Darstellung
  //    text = text.Normalize(NormalizationForm.FormC);

  //    // Ersetze alle Arten von Anführungszeichen
  //    var quoteReplacements = new Dictionary<string, string>
  //      {
  //          {"\u2018", "'"},   // Left Single Quotation Mark
  //          {"\u2019", "'"},   // Right Single Quotation Mark
  //          {"\u0060", "'"},   // Grave Accent
  //          {"\u00B4", "'"},   // Acute Accent

  //          {"\u201C", "\""},  // Left Double Quotation Mark
  //          {"\u201D", "\""},  // Right Double Quotation Mark
  //          {"\u201E", "\""},  // Double Low-9 Quotation Mark
  //          {"\u00AB", "\""},  // Left-Pointing Double Angle Quotation Mark
  //          {"\u00BB", "\""},  // Right-Pointing Double Angle Quotation Mark
  //      };

  //    foreach (var kvp in quoteReplacements)
  //    {
  //      text = text.Replace(kvp.Key, kvp.Value);
  //    }

  //    // Ersetze alle Arten von Binde- und Gedankenstrichen
  //    var dashReplacements = new Dictionary<string, string>
  //      {
  //          {"\u2010", "-"},   // Hyphen
  //          {"\u2011", "-"},   // Non-Breaking Hyphen
  //          {"\u2012", "-"},   // Figure Dash
  //          {"\u2013", "-"},   // En Dash
  //          {"\u2014", "-"},   // Em Dash
  //          {"\u2015", "--"},  // Horizontal Bar
  //      };

  //    foreach (var kvp in dashReplacements)
  //    {
  //      text = text.Replace(kvp.Key, kvp.Value);
  //    }

  //    // Ersetze verschiedene Auslassungspunkte
  //    text = text.Replace("\u2026", "...");
  //    text = text.Replace("\u2025", "...");

  //    // Ersetze verschiedene Whitespace-Zeichen
  //    text = text.Replace("\u00A0", " ");  // Non-breaking space
  //    text = text.Replace("\u2007", " ");  // Figure space
  //    text = text.Replace("\u202F", " ");  // Narrow no-break space

  //    return text;
  //  }

  //  /// <summary>
  //  /// Text in einzelne Wörter tokenisieren
  //  /// </summary>
  //  private static List<string> TokenizeText(string text)
  //  {
  //    if (string.IsNullOrEmpty(text))
  //      return new List<string>();

  //    // Entferne Sonderzeichen, aber behalte Wichtiges für Wortbildung
  //    string cleaned = Regex.Replace(text, @"[^\wäöüÄÖÜß\-]", " ");

  //    // Splitte an Whitespace
  //    var words = cleaned.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

  //    return words.ToList();
  //  }

  //  /// <summary>
  //  /// Wörter filtern basierend auf Kriterien
  //  /// </summary>
  //  private static List<string> FilterWords(List<string> words,
  //      int minWordLength,
  //      bool excludeNumbers,
  //      string[] stopWords)
  //  {
  //    if (stopWords == null)
  //    {
  //      // Standard-Stopwörter für Deutsch
  //      stopWords = new string[]
  //      {
  //              "der", "die", "das", "und", "oder", "aber", "nicht", "ist", "sind", "war",
  //              "waren", "sein", "haben", "hat", "hatte", "hatten", "ich", "du", "er", "sie",
  //              "es", "wir", "ihr", "den", "dem", "des", "in", "auf", "mit", "nach", "von",
  //              "an", "bei", "zu", "als", "am", "um", "bei", "aus", "über", "durch", "gegen",
  //              "seit", "ohne", "gegenüber", "für", "gegen", "bis", "zwischen", "neben",
  //              "sowie", "auch", "nur", "schon", "erst", "noch", "immer", "nie",
  //              "man", "mehr", "weniger", "viel", "vielen", "vielem", "viele", "wenig", "wenigen"
  //      };
  //    }

  //    return words
  //        .Where(w => w.Length >= minWordLength)
  //        .Where(w => !excludeNumbers || !IsNumeric(w))
  //        .Where(w => !stopWords.Contains(w.ToLower()))
  //        .Select(w => CleanWord(w))
  //        .Where(w => !string.IsNullOrEmpty(w))
  //        .ToList();
  //  }

  //  /// <summary>
  //  /// Prüft, ob ein Wort numerisch ist
  //  /// </summary>
  //  private static bool IsNumeric(string word)
  //  {
  //    return double.TryParse(word, out _);
  //  }

  //  /// <summary>
  //  /// Wort bereinigen (Groß-/Kleinschreibung normalisieren)
  //  /// </summary>
  //  private static string CleanWord(string word)
  //  {
  //    // Entferne führende und nachfolgende Sonderzeichen
  //    word = word.Trim();

  //    // Entferne Sonderzeichen am Wortanfang und -ende
  //    word = Regex.Replace(word, @"^[^\wäöüÄÖÜß]+|[^\wäöüÄÖÜß]+$", "");

  //    // Normalisiere auf Lowercase für Konsistenz
  //    word = word.ToLowerInvariant();

  //    return word;
  //  }

  //  /// <summary>
  //  /// Häufigkeit der Wörter ermitteln
  //  /// </summary>
  //  private static Dictionary<string, int> GetKeywordFrequency(List<string> words)
  //  {
  //    return words
  //        .GroupBy(w => w)
  //        .ToDictionary(g => g.Key, g => g.Count());
  //  }

  //}
}