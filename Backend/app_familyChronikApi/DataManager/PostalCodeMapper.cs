using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace app_familyBackend.DataManager
{
  public static class AdressHelperMapper
  {


    public enum ZioRegion
    {
      None,

      // Schweiz (CH)
      CH_AG, CH_AI, CH_AR, CH_BE, CH_BL, CH_BS, CH_FR, CH_GE, CH_GL,
      CH_GR, CH_JU, CH_LU, CH_NE, CH_NW, CH_OW, CH_SG, CH_SH, CH_SO,
      CH_SZ, CH_TG, CH_TI, CH_UR, CH_VD, CH_VS, CH_ZG, CH_ZH,

      // Liechtenstein (LI)
      LI_BALZERS, LI_ESCHEN, LI_GAMPRIN, LI_MAUREN, LI_PLANKEN,
      LI_RUGGELL, LI_SCHAAN, LI_SCHELLENBERG, LI_TRIESEN,
      LI_TRIESENBERG, LI_VADUZ
    }

    public class ParsedAddress
    {
      public string Country { get; set; }
      public string City { get; set; }
      public string Street { get; set; }
      public string HouseNumber { get; set; }
      public string RegionCodeRaw { get; set; }
      public ZioRegion Region { get; set; }
      public string PostalCode { get; set; }
    }

    public static class AddressParser
    {
      private static readonly Regex AddressRegex = new Regex(
          @"^\s*/\s*
          (?<country>[A-Z]{2})\s+
          (?<city>[A-Za-zÄÖÜäöüß]+)
          (?:\s+(?<street>[A-Za-zÄÖÜäöüß0-9\s]+?))?
          (?:\s+Nr\.?\s*(?<housenr>\d+))?
          (?:\s+(?<region>[A-Z]{2}))?
          \s*$",
          RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace
      );



      private static readonly Dictionary<string, ZioRegion> RegionMap =
          new(StringComparer.OrdinalIgnoreCase)
          {
            // Schweiz
            { "AG", ZioRegion.CH_AG }, { "AI", ZioRegion.CH_AI }, { "AR", ZioRegion.CH_AR },
            { "BE", ZioRegion.CH_BE }, { "BL", ZioRegion.CH_BL }, { "BS", ZioRegion.CH_BS },
            { "FR", ZioRegion.CH_FR }, { "GE", ZioRegion.CH_GE }, { "GL", ZioRegion.CH_GL },
            { "GR", ZioRegion.CH_GR }, { "JU", ZioRegion.CH_JU }, { "LU", ZioRegion.CH_LU },
            { "NE", ZioRegion.CH_NE }, { "NW", ZioRegion.CH_NW }, { "OW", ZioRegion.CH_OW },
            { "SG", ZioRegion.CH_SG }, { "SH", ZioRegion.CH_SH }, { "SO", ZioRegion.CH_SO },
            { "SZ", ZioRegion.CH_SZ }, { "TG", ZioRegion.CH_TG }, { "TI", ZioRegion.CH_TI },
            { "UR", ZioRegion.CH_UR }, { "VD", ZioRegion.CH_VD }, { "VS", ZioRegion.CH_VS },
            { "ZG", ZioRegion.CH_ZG }, { "ZH", ZioRegion.CH_ZH },

            // Liechtenstein (Gemeinden)
            { "Balzers",      ZioRegion.LI_BALZERS },
            { "Eschen",       ZioRegion.LI_ESCHEN },
            { "Gamprin",      ZioRegion.LI_GAMPRIN },
            { "Gamprin-Bendern",      ZioRegion.LI_GAMPRIN },
            { "Mauren",       ZioRegion.LI_MAUREN },
            { "Planken",      ZioRegion.LI_PLANKEN },
            { "Ruggell",      ZioRegion.LI_RUGGELL },
            { "Schaan",       ZioRegion.LI_SCHAAN },
            { "Schellenberg", ZioRegion.LI_SCHELLENBERG },
            { "Triesen",      ZioRegion.LI_TRIESEN },
            { "Triesenberg",  ZioRegion.LI_TRIESENBERG },
            { "Vaduz",        ZioRegion.LI_VADUZ }
          };

      private static readonly Dictionary<ZioRegion, string> PostalCodes = new()
    {
        // Liechtenstein
        { ZioRegion.LI_BALZERS, "9496" },
        { ZioRegion.LI_ESCHEN, "9492" },
        { ZioRegion.LI_GAMPRIN, "9487" },
        { ZioRegion.LI_MAUREN, "9493" },
        { ZioRegion.LI_PLANKEN, "9498" },
        { ZioRegion.LI_RUGGELL, "9491" },
        { ZioRegion.LI_SCHAAN, "9494" },
        { ZioRegion.LI_SCHELLENBERG, "9488" },
        { ZioRegion.LI_TRIESEN, "9495" },
        { ZioRegion.LI_TRIESENBERG, "9497" },
        { ZioRegion.LI_VADUZ, "9490" },

        // Schweiz (PLZ-Bereiche)
        { ZioRegion.CH_SO, "4500-4658" },
        { ZioRegion.CH_TG, "8500-8599" }
        // Weitere Kantone nach Bedarf ergänzen
    };

      private static ZioRegion ResolveRegion(string country, string regionCode, string city)
      {
        if (country.Equals("CH", StringComparison.OrdinalIgnoreCase))
        {
          if (regionCode != null && RegionMap.TryGetValue(regionCode, out var region))
            return region;
        }

        if (country.Equals("LI", StringComparison.OrdinalIgnoreCase))
        {
          if (city != null && RegionMap.TryGetValue(city, out var region))
            return region;
        }

        return ZioRegion.None;
      }

      public static ParsedAddress ParseAddress(string input)
      {
        if (string.IsNullOrWhiteSpace(input))
          return null;

        var match = AddressRegex.Match(input);
        if (!match.Success)
          return null;

        var country = match.Groups["country"]?.Value;
        var city = match.Groups["city"]?.Value;
        var regionCode = match.Groups["region"]?.Value;

        var region = ResolveRegion(country, regionCode, city);

        return new ParsedAddress
        {
          Country = country,
          City = city,
          Street = match.Groups["street"]?.Value?.Trim(),
          HouseNumber = match.Groups["housenr"]?.Value,
          RegionCodeRaw = regionCode,
          Region = region,
          PostalCode = PostalCodes.TryGetValue(region, out var plz) ? plz : null
        };
      }
    }

  }
}