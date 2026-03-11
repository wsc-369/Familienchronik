using Microsoft.ML.Data;

namespace app_familyBackend.KI
{
  public class Prediction
  {
    // [ColumnName("PredictedLabel")]
    // public string PredictedAnswer { get; set; }

    [ColumnName("PredictedPersonNr")]
    public int PredictedPersonNr { get; set; }
  }
}
