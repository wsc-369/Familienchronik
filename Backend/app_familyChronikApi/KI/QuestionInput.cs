using Microsoft.ML.Data;

namespace app_familyBackend.KI
{
  public class QuestionInput
  {
    [LoadColumn(0)]
    public string Question { get; set; }

  }
}
