using appAhnenforschungBackEnd.Models;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using appAhnenforschungData.Models.DB;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace app_familyBackend.KI
{
  public class KIManager
  {

    private wsc_chronikContext db = new wsc_chronikContext();
    private CSettings _oSettings;
    private string _csvFilePath;
    private string _modelPath;


    public void Init(CSettings i_oSettings)
    {
      _oSettings = i_oSettings;
      _csvFilePath = "ChronikTraining_data.csv";
      _modelPath = "ChronikQuestion_answering_model.zip";
    }

    public void GenerateTrainingDataFromDatabase(CPerson p)
    {
      try
      {
        List<PersonData> persons = GetPersonDataFromDatabase();
        var csvContent = new StringBuilder();
        csvContent.AppendLine("Question,Answer,PersonNr,PartnerPersonNr");

        foreach (var person in persons)
        {
          string question = "Welche Informationen gibt es über die Personen in der Datenbank?";
          string answer = FormatPersonData(person);

          csvContent.AppendLine($"\"{question}\"|\"{answer}\"|{person.PersonNr}|{person.PartnerNr}");
          //csvContent.AppendLine($"\"{question}\"|\"{answer}\"|\"{person.PersonNr}\"|\"{person.PartnerNr}\"");

          question = "Welche Informationen gibt es über " + person.Vorname + " " + person.Name + "?";
          csvContent.AppendLine($"\"{question}\"|\"{answer}\"|{person.PersonNr}|{person.PartnerNr}");
          //csvContent.AppendLine($"\"{question}\"|\"{answer}\"|\"{person.PersonNr}\"|\"{person.PartnerNr}\"");

          //question = person.Vorname + " " + person.Name + "?";
          //csvContent.AppendLine($"\"{question}\",\"{answer}\",\"{person.PersonNr}\",\"{person.PartenrNr}\"");

          //question = person.Vorname + " " + person.Name + "?";
          //csvContent.AppendLine($"\"{question}\",\"{answer}\",\"{person.PersonNr}\",\"{person.PartenrNr}\"");

          //question = ($"Welche Informationen gibt es über {person.Vorname} {person.Name} geboren am {person.Geburtsdatum}, gestoben am {person.Sterbedatum}");
          //csvContent.AppendLine($"\"{question}\",\"{answer}\"");

          //question = ($"Erzählen Sie über die Familie {person.Vorname} {person.Name} geboren am {person.Geburtsdatum}, gestoben am {person.Sterbedatum}");
          //csvContent.AppendLine($"\"{question}\",\"{answer}\"");


          //question = ($"Welche Lebensdaten gibt es über {person.Vorname} {person.Name} geboren am {person.Geburtsdatum}, gestoben am {person.Sterbedatum}");
          //csvContent.AppendLine($"\"{question}\",\"{answer}\"");

          //question = ($"Welche Familieninformationen gibt es über {person.Vorname} {person.Name} geboren am {person.Geburtsdatum}, gestoben am {person.Sterbedatum}");
          //csvContent.AppendLine($"\"{question}\",\"{answer}\"");

          //question = ($"Welche Familieninformationen gibt es über {person.Vorname} {person.Name} geboren am {person.Geburtsdatum}, gestoben am {person.Sterbedatum}");
          //csvContent.AppendLine($"\"{question}\",\"{answer}\"");

          //question = ($"{person.Vorname} {person.Name} geboren am {person.Geburtsdatum}, gestoben am {person.Sterbedatum}");
          //csvContent.AppendLine($"\"{question}\",\"{answer}\"");
        }

        File.WriteAllText(_csvFilePath, csvContent.ToString(), Encoding.UTF8);

      }
      catch (Exception ex)
      {
        Console.WriteLine($"Fehler bei der CSV-Generierung: {ex.Message}");
      }
    }

    private List<PersonData> GetPersonDataFromDatabase(CPerson p)
    {
      var personData = new List<PersonData>();
      var oRead = new CReadWriteData();
      var oPersons = new List<CPerson>();
      var oPartners = new List<CPartner>();
      var oPersonPersons = new List<CPerson>();

      oPersons.Add(oRead.GetPersonByID(p.PersonID, _oSettings));

      var partners = string.Empty;

      foreach (var person in oPersons)
      {
        oPartners = oRead.GetPartnersByPersonID(person.PersonID, _oSettings);
        foreach (var partner in oPartners)
        {
          oPersonPersons.Add(oRead.GetPersonByID(partner.PersonID, _oSettings));
          continue;
        }

        personData.Add(new PersonData
        {
          Name = person.FamilyName,
          Vorname = person.FirstName,
          Geburtsdatum = person.BirthDisplay,
          Sterbedatum = person.IsDeath ? person.DeathDisplay : string.Empty,
          Heiratsdatum = oPartners.Any() ? oPartners.First().MarriageDateDisplay : string.Empty,
          PartnerVorname = oPartners.Any() ? oPersonPersons.First().FirstName : string.Empty,
          PartnerName = oPartners.Any() ? oPersonPersons.First().FamilyName : string.Empty,
          PartnerGeburtsdatum = oPartners.Any() ? oPersonPersons.First().BirthDisplay : string.Empty,
          PartnerSterbedatum = oPartners.Any() ? oPersonPersons.First().DeathDisplay : string.Empty,
          PersonNr = int.Parse(person.PersonID.Where(char.IsDigit).ToArray()),
          PartnerNr = oPartners.Any() ? int.Parse(oPersonPersons.First().PersonID.Where(char.IsDigit).ToArray()) : 0,
        });
        oPersonPersons.Clear();
      }
      return personData;
    }

    private List<PersonData> GetPersonDataFromDatabase()
    {
      var personData = new List<PersonData>();
      var oRead = new CReadWriteData();
      var oPartners = new List<CPartner>();
      var oPersonPersons = new List<CPerson>();

      var (persons, tpersons) = oRead.GetPersonsIncludedTable(_oSettings);

      var partners = string.Empty;

      foreach (var person in persons)
      {
        if (person.PersonID.Length != 0 && person.FamilyName.Length != 0 && person.FirstName.Length != 0)
        {
          oPartners = oRead.GetPartnersByPersonID(person.PersonID, tpersons, _oSettings);
          if (oPartners.Count == 0)
          {
            personData.Add(new PersonData
            {
              Name = person.FamilyName,
              Vorname = person.FirstName,
              Geburtsdatum = person.BirthDate != CGlobal.CliensSideEmptyDate() ? person.BirthDisplay : string.Empty,
              Sterbedatum = person.DeathDate != CGlobal.CliensSideEmptyDate() ? person.DeathDisplay : string.Empty,
              Heiratsdatum = string.Empty,
              PartnerVorname = string.Empty,
              PartnerName = string.Empty,
              PartnerGeburtsdatum = string.Empty,
              PartnerSterbedatum = string.Empty,
              PersonNr = int.Parse(person.PersonID.Where(char.IsDigit).ToArray()),
              PartnerNr = 0

            });
          }
          else
          {
            foreach (var partner in oPartners)
            {
              oPersonPersons.Add(oRead.GetPersonByID(partner.PersonID, tpersons, _oSettings));
              continue;
            }

            personData.Add(new PersonData
            {
              Name = person.FamilyName,
              Vorname = person.FirstName,
              Geburtsdatum = person.BirthDate != CGlobal.CliensSideEmptyDate() ? person.BirthDisplay : string.Empty,
              Sterbedatum = person.DeathDate != CGlobal.CliensSideEmptyDate() ? person.DeathDisplay : string.Empty,
              Heiratsdatum = oPartners.Any() ? oPartners.First().MarriageDateDisplay : string.Empty,
              PartnerVorname = oPartners.Any() ? oPersonPersons.First().FirstName : string.Empty,
              PartnerName = oPartners.Any() ? oPersonPersons.First().FamilyName : string.Empty,
              PartnerGeburtsdatum = oPartners.Any() && oPersonPersons.First().BirthDate != CGlobal.CliensSideEmptyDate() ? oPersonPersons.First().BirthDisplay : string.Empty,
              PartnerSterbedatum = oPartners.Any() && oPersonPersons.First().DeathDate != CGlobal.CliensSideEmptyDate() ? oPersonPersons.First().DeathDisplay : string.Empty,
              PersonNr =  int.Parse(person.PersonID.Where(char.IsDigit).ToArray()),
              PartnerNr = oPartners.Any() ? int.Parse(oPartners.First().PersonID.Where(char.IsDigit).ToArray()): 0,
            });
            oPersonPersons.Clear();
          }
        }
      }
      return personData;
    }

    private static string FormatPersonData(PersonData person)
    {
      StringBuilder result = new StringBuilder();
      result.Append($"{person.Vorname} {person.Name}");

      if (!string.IsNullOrEmpty(person.Geburtsdatum))
      {
        result.Append($", geboren am {person.Geburtsdatum}");
      }

      if (!string.IsNullOrEmpty(person.Sterbedatum))
      {
        result.Append($", gestoben am {person.Sterbedatum}");
      }

      if (!string.IsNullOrEmpty(person.Heiratsdatum))
      {
        result.Append($", verheiratet am {person.Heiratsdatum}");
      }

      if (!string.IsNullOrEmpty(person.PartnerName))
      {
        result.Append($" mit {person.PartnerVorname} {person.PartnerName}");

        if (!string.IsNullOrEmpty(person.PartnerGeburtsdatum))
        {
          result.Append($" geboren am {person.PartnerGeburtsdatum}");
        }

        if (!string.IsNullOrEmpty(person.PartnerSterbedatum))
        {
          result.Append($", gestoben am {person.PartnerSterbedatum}");
        }

        //if (!string.IsNullOrEmpty(person.PartnerHeiratsdatum))
        //{
        //  result.Append($", verheiratet am {person.PartnerHeiratsdatum}");
        //}
      }

      result.Append(".");
      return result.ToString();
    }

    public async void TrainModel()
    {
      try
      {
        var mlContext = new MLContext();

        // Daten laden
        var data = mlContext.Data.LoadFromTextFile<QuestionAnswerData>(_csvFilePath, hasHeader: true, separatorChar: '|');


        // Pipeline: Frage → Features → Klassifikation auf PersonNr
        var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(QuestionAnswerData.Question))
            .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(QuestionAnswerData.PersonNr)))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedPersonNr", "PredictedLabel"));

        // Trainieren
        var model = pipeline.Fit(data);

        // Modell speichern
        mlContext.Model.Save(model, data.Schema, _modelPath);

        //// Prediction Engine
        //var predEngine = mlContext.Model.CreatePredictionEngine<QuestionAnswerData, Prediction>(model);

        //// Beispiel: Frage stellen
        //var input = new QuestionAnswerData { Question = "Welche Informationen gibt es über Martino ?" };
        //var prediction = predEngine.Predict(input);

        //Console.WriteLine($"Vorhergesagte PersonNr: {prediction.PredictedPersonNr}");

        //// Lookup: Antwort und PartnerPersonNr aus Originaldaten holen
        //var rows = mlContext.Data.CreateEnumerable<QuestionAnswerData>(data, reuseRowObject: false);
        //var matched = rows.FirstOrDefault(r => r.PersonNr == prediction.PredictedPersonNr);

        //if (matched != null)
        //{
        //  Console.WriteLine($"Antwort: {matched.Answer}");
        //  Console.WriteLine($"PartnerPersonNr: {matched.PartnerPersonNr}");
        //}
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Fehler beim Training: {ex.Message}");
      }
    }

    public async Task QueryModel(CPerson p)
    {
      try
      {
        var mlContext = new MLContext();

        // Modell laden
        ITransformer trainedModel;
        DataViewSchema modelSchema;
        trainedModel = mlContext.Model.Load(_modelPath, out modelSchema);

        // Daten laden
        var data = mlContext.Data.LoadFromTextFile<QuestionAnswerData>(_csvFilePath, hasHeader: true, separatorChar: '|');
        // PredictionEngine für Einzelvorhersagen erstellen
        var predEngine = mlContext.Model.CreatePredictionEngine<QuestionAnswerData, Prediction>(trainedModel);

        // Prediction Engine
       // var predEngine = mlContext.Model.CreatePredictionEngine<QuestionAnswerData, Prediction>(model);

        // Beispiel: Frage stellen
        var input = new QuestionAnswerData { Question = "Welche Informationen gibt es über " + p.Fullname + "?" };
        var prediction = predEngine.Predict(input);

        Console.WriteLine($"Vorhergesagte PersonNr: {prediction.PredictedPersonNr}");

        // Lookup: Antwort und PartnerPersonNr aus Originaldaten holen
        var rows = mlContext.Data.CreateEnumerable<QuestionAnswerData>(data, reuseRowObject: false);
        var matched = rows.FirstOrDefault(r => r.PersonNr == prediction.PredictedPersonNr);

        if (matched != null)
        {
          Console.WriteLine($"Antwort: {matched.Answer}");
          Console.WriteLine($"PartnerPersonNr: {matched.PartnerPersonNr}");
        }

        //// Beispiel-Eingabe
        //var sample = new QuestionAnswerData
        //{
        //  Question = "Welche Informationen gibt es über " + p.FirstName + " " + p.PreName + "?",
        //  PersonNr = int.Parse( p.PersonID.Where(char.IsDigit).ToArray()),
        //  PartnerPersonNr = 0
        //};

        //// Vorhersage abrufen
        //var prediction = predEngine.Predict(sample);

       // Console.WriteLine($"Vorhersage: {prediction.PredictedAnswer}");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Fehler bei Vorhersage: {ex.Message}");
      }
    }
    //public string TestModel(string question = "Welche Informationen gibt es über die Personen in der Datenbank?")
    //{
    //  try
    //  {
    //    var mlContext = new MLContext();

    //    ITransformer trainedModel;
    //    DataViewSchema modelSchema;
    //    // Modell laden
    //    var trainedModel = mlContext.Data.LoadFromTextFile<QuestionAnswerData>(_csvFilePath, hasHeader: true, separatorChar: ',');

    //    // Testfrage stellen
    //    string testQuestion = question;

    //    // Vorhersage machen
    //    var predictionEngine = mlContext.Model.CreatePredictionEngine<QuestionInput, QuestionPrediction>(trainedModel);
    //    var input = new QuestionInput { Question = testQuestion };
    //    var prediction = predictionEngine.Predict(input);

    //    Console.WriteLine($"Frage: {testQuestion}");
    //    Console.WriteLine($"Antwort: {prediction.Answer}");

    //    return prediction.Answer;
    //  }
    //  catch (Exception ex)
    //  {
    //    Console.WriteLine($"Fehler beim Testen: {ex.Message}");
    //    return string.Empty;
    //  }
    //}





  }
}
