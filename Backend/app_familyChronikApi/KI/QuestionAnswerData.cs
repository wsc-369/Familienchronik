using Microsoft.ML.Data;

namespace app_familyBackend.KI
{
  public class QuestionAnswerData
  {
    [LoadColumn(0)]
    public string Question { get; set; }

    [LoadColumn(1)]
    public string Answer { get; set; }

    [LoadColumn(2)]
    public int PersonNr { get; set; }

    [LoadColumn(3)]
    public int PartnerPersonNr { get; set; }

  }
}
