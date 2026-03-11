using System;

namespace app_familyBackend.KI
{
  public class HtmlGenerator
  {
    public static string GenerateHtmlWithImage(string personId, string imageType = "profile")
    {
      string template = @"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Bild-Anzeige für Person {0}</title>
        </head>
        <body>
            <h1>Bild der Person</h1>
            <img src=""images/person_{0}_{1}.jpg"" alt=""Bild der Person ID: {0}"">
            <p>Person ID: {0}</p>
            <p>Bildtyp: {1}</p>
        </body>
        </html>";

      return string.Format(template, personId, imageType);
    }

    public static void MainTest()
    {
      string htmlOutput = GenerateHtmlWithImage("12345", "profile");
      Console.WriteLine(htmlOutput);
    }

  }
}
