using Microsoft.ML.Data;

namespace app_familyBackend.KI
{
  public class QuestionPrediction
  {
    [ColumnName("PredictedLabel")]
    public string Answer { get; set; }

  }
}
