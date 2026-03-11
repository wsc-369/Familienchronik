using System.Collections.Generic;

namespace ValueObject
{
  public static class JsonLikeHelper
  {
    /// <summary>
    /// Baut ein LIKE-Pattern für ein JSON-Feld.
    /// </summary>
    /// <param name="jsonKey">Der JSON-Key, z.B. "FirstName".</param>
    /// <param name="value">Der Suchwert, z.B. "Walter".</param>
    /// <param name="wildcardBefore">Ob ein % vor dem Wert gesetzt wird.</param>
    /// <param name="wildcardAfter">Ob ein % nach dem Wert gesetzt wird.</param>
    /// <returns>LIKE-Pattern für EF.Functions.Like</returns>
    public static IEnumerable<string> BuildJsonLikeVariants(
            string jsonKey,
            string value,
            bool wildcardBefore = false,
            bool wildcardAfter = true)
    {
      if (string.IsNullOrEmpty(value))
        return new[] { "%" };

      var before = wildcardBefore ? "%" : "";
      var after = wildcardAfter ? "%" : "";

      // Normale Schreibweise
      var normal = $"%\"{jsonKey}\":\"{before}{value}{after}\"%";

      // Unicode-Escape Variante (ä -> \u00E4 usw.)
      var escapedValue = value
          .Replace("ä", "\\u00E4")
          .Replace("Ä", "\\u00C4")
          .Replace("ö", "\\u00F6")
          .Replace("Ö", "\\u00D6")
          .Replace("ü", "\\u00FC")
          .Replace("Ü", "\\u00DC")
          .Replace("ß", "\\u00DF");

      var escaped = $"%\"{jsonKey}\":\"{before}{escapedValue}{after}\"%";

      return new[] { normal, escaped };
    }
  }

}
