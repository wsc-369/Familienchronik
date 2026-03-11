using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace app_familyBackend.KI
{

//  1. Hauptdaten.csv:
//  Frage,Antwort,PersonId,person_id,image_path,description
//  "Was ist dein Name?","Ich bin Max","12345","12345","images/person_12345.jpg","Profile Bild"

//  2. Weiterfuehrende_Fragen.csv:
//  PersonId,Frage
//  12345,"Was machst du beruflich?"
//  12345,"Woher kommst du?"
//  67890,"Welche Städte hast du bereist?"

  public class CsvDataMerger
  {
    public static void MergeCsvFiles(string file1, string file2, string outputFile)
    {
      // CSV-Datei 1 lesen
      var data1 = ReadCsvFile(file1);
      // CSV-Datei 2 lesen
      var data2 = ReadCsvFile(file2);

      // Daten zusammenführen
      var mergedData = new List<Dictionary<string, string>>();

      // Beispiel für Zusammenführung anhand eines Schlüssels
      foreach (var row1 in data1)
      {
        var mergedRow = new Dictionary<string, string>(row1);

        // Suchen Sie nach entsprechenden Daten in der zweiten Datei
        var matchingRow = data2.FirstOrDefault(row2 =>
            row2.ContainsKey("PersonId") &&
            row2["PersonId"] == row1["PersonId"]);

        if (matchingRow != null)
        {
          foreach (var kvp in matchingRow)
          {
            if (!mergedRow.ContainsKey(kvp.Key))
            {
              mergedRow[kvp.Key] = kvp.Value;
            }
          }
        }

        mergedData.Add(mergedRow);
      }

      // Ergebnis in neue CSV-Datei schreiben
      WriteCsvFile(outputFile, mergedData);
    }

    private static List<Dictionary<string, string>> ReadCsvFile(string filePath)
    {
      var data = new List<Dictionary<string, string>>();
      var lines = File.ReadAllLines(filePath);

      if (lines.Length == 0) return data;

      var headers = lines[0].Split(',');

      for (int i = 1; i < lines.Length; i++)
      {
        var values = lines[i].Split(',');
        var row = new Dictionary<string, string>();

        for (int j = 0; j < headers.Length; j++)
        {
          row[headers[j]] = values[j];
        }

        data.Add(row);
      }

      return data;
    }

    private static void WriteCsvFile(string filePath, List<Dictionary<string, string>> data)
    {
      if (data.Count == 0) return;

      var headers = data[0].Keys.ToArray();
      var lines = new List<string>();

      // Kopfzeile
      lines.Add(string.Join(",", headers));

      // Datenzeilen
      foreach (var row in data)
      {
        var values = headers.Select(header => row.ContainsKey(header) ? row[header] : "").ToArray();
        lines.Add(string.Join(",", values));
      }

      File.WriteAllLines(filePath, lines);
    }

  }
}

