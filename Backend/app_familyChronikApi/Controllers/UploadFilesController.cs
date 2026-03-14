using app_familyBackend.DataManager;
using app_familyChronikApi.ReadWriteDB;
using appAhnenforschungBackEnd.Configutation;
using appAhnenforschungBackEnd.Models;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using static appAhnenforschungBackEnd.DataManager.CGlobal;
using static appAhnenforschungData.Models.App.CContentTemplate;

// https://www.c-sharpcorner.com/article/upload-download-files-in-asp-net-core-2-0/
namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UploadFilesController : ControllerBase
  {
    private readonly ILogger<UploadFilesController> _logger;
    private readonly ReadWirteContents _readWirteContents;
    private readonly CSettings _oSettings;
    public UploadFilesController(ReadWirteContents readWirteContents, ILogger<UploadFilesController> logger)
    {
      _logger = logger;
      _oSettings = new CSettings();
      _oSettings.UrlImagePath = ReadSettings.UrlRessources();
      _readWirteContents= readWirteContents;
    }

    /// <summary>
    /// Alle Bilder für das Bilderkarusel auf der Startseite.
    /// </summary>
    /// <returns>Ein Lister der Bilder</returns>
    [HttpGet("getImagesSlideMainPage")]
    public async Task<IEnumerable<ValueObject.Images>> GetImagesSlideMainPage()
    {
      int index = 0;
      List<ValueObject.Images> arlImages = new List<ValueObject.Images>();

      var folderName = Path.Combine("resources", "images");
      folderName = Path.Combine(folderName, ETemplateTypes.mainPageSlide.ToString());

      var pathReading = Path.Combine(Directory.GetCurrentDirectory(), folderName);

      CReadWriteData oReadWriteData = new CReadWriteData();
      var arlIContentTemplateImages = await _readWirteContents.GetContendTemplateImagesByType(TemplateTypes.mainPageSlide, true);

      if (Directory.Exists(pathReading) && arlIContentTemplateImages != null)
      {
        string[] fileEntries = Directory.GetFiles(Path.Combine(pathReading, "original"));
        foreach (string fileName in fileEntries)
        {
          FileInfo oFileInfo = new FileInfo(fileName);
          var img = new ValueObject.Images();
          img.index = index;
          img.urlLarge = _oSettings.UrlImagePath + "images/" + ETemplateTypes.mainPageSlide.ToString() + "/large/" + oFileInfo.Name;
          img.urlSmall = _oSettings.UrlImagePath + "images/" + ETemplateTypes.mainPageSlide.ToString() + "/small/" + oFileInfo.Name;
          img.urlThumb = _oSettings.UrlImagePath + "images/" + ETemplateTypes.mainPageSlide.ToString() + "/thumb/" + oFileInfo.Name;

          var contentTemplateImage = arlIContentTemplateImages.FirstOrDefault(p => p.ImageName == oFileInfo.Name);
          if (contentTemplateImage != null)
          {
            img.title = contentTemplateImage.Title;
            img.subTitle = contentTemplateImage.SubTitle;
            img.description = contentTemplateImage.Description;
            img.type = TemplateTypes.mainPageSlide;
            img.sortNo = contentTemplateImage.SortNo;
            arlImages.Add(img);
            index = index + 1;
          }
        }
      }

      return arlImages;
    }

    [HttpGet("{id}")]
    public IActionResult GetPath(string id)
    {
      var dbPath = "";
      switch (id)
      {
        case "Root":
          dbPath = Directory.GetCurrentDirectory();
          break;
        case "images":
          var folderName = _oSettings.UrlImagePath + "/images/";
          //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
          dbPath = folderName;// pathToSave;
          break;
        default:
          Console.WriteLine("Default case");
          break;
      }
      return Ok(new { dbPath });
    }


    //[HttpPost(Name = "uploadMulitpleFiles"), DisableRequestSizeLimit]
    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost, DisableRequestSizeLimit]
    public IActionResult Upload()
    {
      try
      {
        var file = Request.Form.Files[0];
        var folderName = Path.Combine("resources", "images");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var fullPath = Path.Combine(pathToSave, fileName);
          var dbPath = Path.Combine(folderName, fileName);

          using (var stream = new FileStream(fullPath, FileMode.Create))
          {
            file.CopyTo(stream);
          }

          return Ok(new { dbPath });
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }

    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("Upload/{id}"), DisableRequestSizeLimit]
    public IActionResult Upload(string id)
    {
      try
      {
        var file = Request.Form.Files[0];
        var folderName = Path.Combine("resources", "images");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var extention = Path.GetExtension(fileName);

          fileName = id + extention;

          var fullPath = Path.Combine(pathToSave, fileName);
          var dbPath = Path.Combine(folderName, fileName);

          using (var stream = new FileStream(fullPath, FileMode.Create))
          {
            file.CopyTo(stream);
          }

          return Ok(new { dbPath });
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }


    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("UploadImagePerson/{id}"), DisableRequestSizeLimit]
    public IActionResult UploadImagePerson(string id)
    {
      try
      {
        var file = Request.Form.Files[0];
        var folderName = Path.Combine("resources", "images");
        folderName = Path.Combine(folderName, "pe");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var extention = Path.GetExtension(fileName);

          CReadWriteData oReadWriteData = new CReadWriteData();
          CPerson person = oReadWriteData.GetPersonByID(id, DataManager.CGlobal.Settings());
          fileName = person.PersonID.Replace("I", "") + "_" + person.FirstName + "_" + person.FamilyName + "_PE_01" + extention;

          string strImagePathOriginal = Path.Combine(pathToSave, CImages.EFileDirectory.original.ToString());
          string strImagePathLarge = Path.Combine(pathToSave, "160x200");
          string strImageFileSmall = Path.Combine(pathToSave, "80x100");
          string strImagePathThumb = Path.Combine(pathToSave, "48x60");

          if (!Directory.Exists(pathToSave))
          {
            Directory.CreateDirectory(pathToSave);
            Directory.CreateDirectory(strImagePathOriginal);
            Directory.CreateDirectory(strImagePathLarge);
            Directory.CreateDirectory(strImageFileSmall);
            Directory.CreateDirectory(strImagePathThumb);
          }

          strImagePathOriginal = Path.Combine(strImagePathOriginal, fileName);
          strImagePathLarge = Path.Combine(strImagePathLarge, fileName);
          strImageFileSmall = Path.Combine(strImageFileSmall, fileName);
          strImagePathThumb = Path.Combine(strImagePathThumb, fileName);

          using (var stream = new FileStream(strImagePathOriginal, FileMode.Create))
          {
            file.CopyTo(stream);
          }

          (new ImagesHelper()).ResizePersonalImage(strImagePathOriginal, strImagePathLarge, strImageFileSmall, strImagePathThumb);

          //Image image = Image.Load(fullPath);
          //for (int i = 1; i < 4; i++)
          //{
          //  switch (i)
          //  {
          //    case 1:
          //      image = Image.Load(fullPath);
          //      image.Mutate(ctx => ctx.Resize(160, 200)); // resize the image in place and return it for chaining
          //      image.Save(strImagePathLarge); // based on the file extension pick an encoder then encode and write the data to disk
          //      break;
          //    case 2:
          //      image = Image.Load(fullPath);
          //      image.Mutate(ctx => ctx.Resize(80, 100));
          //      image.Save(strImageFileSmall); 
          //      break;
          //    case 3:
          //      image = Image.Load(fullPath);
          //      image.Mutate(ctx => ctx.Resize(46, 60));
          //      image.Save(strImagePathThumb);
          //      break;
          //    default:
          //      Console.WriteLine("Default case");
          //      break;
          //  }
          //  //person.ImagePath = strImageFileNormal;
          //  //person.ImagePathSmall = strImagePathSmall;
          //  //person.ImagePathBig = strImagePathBig;
          //  //oReadWriteData.UpdatePerson(person, DataManager.CGlobal.Settings());

          //
          return Ok(new { strImagePathOriginal });
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception)
      {
        return StatusCode(500, "Internal server error");
      }
    }


    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("UploadImagePortrait/{id}"), DisableRequestSizeLimit]
    public IActionResult UploadImagePortrait(string id)
    {
      try
      {
        var file = Request.Form.Files[0];
        var folderName = Path.Combine("resources", "documents");
        folderName = Path.Combine(folderName, "portraits");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        if (!Directory.Exists(pathToSave))
        {
          Directory.CreateDirectory(pathToSave);
        }

        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var extention = Path.GetExtension(fileName);

          CReadWriteData oReadWriteData = new CReadWriteData();
          CPerson person = oReadWriteData.GetPersonByID(id, DataManager.CGlobal.Settings());
          fileName = person.PersonID.Replace("I", "") + "_" + fileName; // person.FirstName + "_" + person.PreName + "_Portrait" + extention;

          var fullPath = Path.Combine(pathToSave, fileName);
          var dbPath = Path.Combine(folderName, fileName);
          using (var stream = new FileStream(fullPath, FileMode.Create))
          {
            file.CopyTo(stream);
          }

          return Ok(new { dbPath });
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }

    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("UploadImageMainSlides"), DisableRequestSizeLimit]
    public IActionResult UploadImageMainSlides()
    {
      try
      {
        var file = Request.Form.Files[0];
        var folderName = Path.Combine("resources", "images");
        folderName = Path.Combine(folderName, "mainSlide");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        string strImagePathOriginal = Path.Combine(pathToSave, CImages.EFileDirectory.original.ToString());
        string strImagePathLarge = Path.Combine(pathToSave, CImages.EFileDirectory.large.ToString());
        string strImageFileSmall = Path.Combine(pathToSave, CImages.EFileDirectory.small.ToString());
        string strImagePathThumb = Path.Combine(pathToSave, CImages.EFileDirectory.thumb.ToString());


        if (!Directory.Exists(pathToSave))
        {
          Directory.CreateDirectory(pathToSave);
          Directory.CreateDirectory(strImagePathOriginal);
          Directory.CreateDirectory(strImagePathLarge);
          Directory.CreateDirectory(strImageFileSmall);
          Directory.CreateDirectory(strImagePathThumb);
        }

        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var extention = Path.GetExtension(fileName);

          strImagePathOriginal = Path.Combine(strImagePathOriginal, fileName);
          strImagePathLarge = Path.Combine(strImagePathLarge, fileName);
          strImageFileSmall = Path.Combine(strImageFileSmall, fileName);
          strImagePathThumb = Path.Combine(strImagePathThumb, fileName);

          using (var stream = new FileStream(strImagePathOriginal, FileMode.Create))
          {
            file.CopyTo(stream);
          }

          (new ImagesHelper()).Resize(strImagePathOriginal, strImagePathLarge, strImageFileSmall, strImagePathThumb);

          return Ok(new { strImagePathOriginal });
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }

    [Authorize(Roles = AppUserRoleAdmin)]
    [HttpPost("UploadDialectAudio"), DisableRequestSizeLimit]
    public async Task<IActionResult> UploadDialectAudio()
    {
      try
      {
        var file = Request.Form.Files[0];
        var folderName = Path.Combine("resources", "audio");
        folderName = Path.Combine(folderName, "dialect");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        string strImagePathOriginal = Path.Combine(pathToSave, CImages.EFileDirectory.original.ToString());

        if (!Directory.Exists(pathToSave))
        {
          Directory.CreateDirectory(pathToSave);
        }

        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var extention = Path.GetExtension(fileName);

          var fullPath = Path.Combine(pathToSave, fileName);
         
          if (!System.IO.File.Exists(fullPath))
          {
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
              file.CopyTo(stream);
            }
          }
          else
          {
            return Ok(new { fileName }); ;
          }

          var dbPath = Path.Combine(folderName, fileName);
          
          return Ok(new { dbPath });
         
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }


    /// <summary>
    /// Ein WIF Bild hochladen und in verschiedenen Groessen verteilen
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("UploadImagePersonRow/{id}"), DisableRequestSizeLimit]
    public IActionResult UploadImagePersonRow(int id)
    {
      try
      {
        var file = Request.Form.Files[0];
        var httpPath = _oSettings.UrlImagePath + "/images/imagePerson/imagePersonRow";

        var folderName = Path.Combine("resources", "images");
        folderName = Path.Combine(folderName, "imagePerson");
        folderName = Path.Combine(folderName, "imagePersonRow");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        string strImagePathOriginal = Path.Combine(pathToSave, CImages.EFileDirectory.original.ToString());
        string strImagePathLarge = Path.Combine(pathToSave, CImages.EFileDirectory.large.ToString());
        string strImageFileSmall = Path.Combine(pathToSave, CImages.EFileDirectory.small.ToString());
        string strImagePathThumb = Path.Combine(pathToSave, CImages.EFileDirectory.thumb.ToString());


        if (!Directory.Exists(pathToSave))
        {
          Directory.CreateDirectory(pathToSave);
          Directory.CreateDirectory(strImagePathOriginal);
          Directory.CreateDirectory(strImagePathLarge);
          Directory.CreateDirectory(strImageFileSmall);
          Directory.CreateDirectory(strImagePathThumb);
        }

        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var extention = Path.GetExtension(fileName);

          strImagePathOriginal = Path.Combine(strImagePathOriginal, fileName);
          strImagePathLarge = Path.Combine(strImagePathLarge, fileName);
          strImageFileSmall = Path.Combine(strImageFileSmall, fileName);
          strImagePathThumb = Path.Combine(strImagePathThumb, fileName);

          using (var stream = new FileStream(strImagePathOriginal, FileMode.Create))
          {
            file.CopyTo(stream);
          }

            (new ImagesHelper()).Resize(strImagePathOriginal, strImagePathLarge, strImageFileSmall, strImagePathThumb);

          CReadWriteData oReadWriteData = new CReadWriteData();
          CImagePerson imagePerson = oReadWriteData.GetImagePersonsById(id, false);
          imagePerson.OriginalFileName = file.FileName;
          imagePerson.ImagePath = httpPath;
          oReadWriteData.CreateOrUpdateImagePersons(imagePerson);
          return Ok(new { strImagePathOriginal });
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }


    /// <summary>
    /// Ein WIF Bild das nummeriert ist, hochladen und in verschiedenen Groessen verteilen
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "EditAdress, EditMainPage, Admin")]
    [HttpPost("UploadImagePersonNum/{id}"), DisableRequestSizeLimit]
    public IActionResult UploadImagePersonWithNumbers(int id)
    {
      try
      {
        var file = Request.Form.Files[0];
        var httpPath = _oSettings.UrlImagePath + "/images/imagePerson/imagePersonNum";
        var folderName = Path.Combine("resources", "images");
        folderName = Path.Combine(folderName, "imagePerson");
        folderName = Path.Combine(folderName, "imagePersonNum");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        string strImagePathOriginal = Path.Combine(pathToSave, CImages.EFileDirectory.original.ToString());
        string strImagePathLarge = Path.Combine(pathToSave, CImages.EFileDirectory.large.ToString());
        string strImageFileSmall = Path.Combine(pathToSave, CImages.EFileDirectory.small.ToString());
        string strImagePathThumb = Path.Combine(pathToSave, CImages.EFileDirectory.thumb.ToString());


        if (!Directory.Exists(pathToSave))
        {
          Directory.CreateDirectory(pathToSave);
          Directory.CreateDirectory(strImagePathOriginal);
          Directory.CreateDirectory(strImagePathLarge);
          Directory.CreateDirectory(strImageFileSmall);
          Directory.CreateDirectory(strImagePathThumb);
        }

        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var extention = Path.GetExtension(fileName);

          strImagePathOriginal = Path.Combine(strImagePathOriginal, fileName);
          strImagePathLarge = Path.Combine(strImagePathLarge, fileName);
          strImageFileSmall = Path.Combine(strImageFileSmall, fileName);
          strImagePathThumb = Path.Combine(strImagePathThumb, fileName);

          using (var stream = new FileStream(strImagePathOriginal, FileMode.Create))
          {
            file.CopyTo(stream);
          }

            (new ImagesHelper()).Resize(strImagePathOriginal, strImagePathLarge, strImageFileSmall, strImagePathThumb);

          CReadWriteData oReadWriteData = new CReadWriteData();
          CImagePerson imagePerson = oReadWriteData.GetImagePersonsById(id, false);
          imagePerson.FileName = file.FileName;
          imagePerson.ImagePath = httpPath;
          oReadWriteData.CreateOrUpdateImagePersons(imagePerson);

          return Ok(new { strImagePathOriginal });
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }

    /// <summary>
    /// Alle Bilder für das Bilderkarusel auf der Startseite.
    /// </summary>
    /// <returns>Ein Lister der Bilder</returns>
    [HttpGet("getFilesByContentTemplateType/{id}")]
    public IEnumerable<CImages> GetFilesByContentTemplateType([FromRoute] int id)
    {
      CContentTemplate.ETemplateTypes type = (CContentTemplate.ETemplateTypes)id;
      int index = 0;
      List<CImages> arlImages = new List<CImages>();

      var folderName = Path.Combine("resources", "images");

      ImagesHelper imagesHelper = new ImagesHelper();
      CReadWriteData oReadWriteData = new CReadWriteData();
      List<CContentTemplateImage> arlIContentTemplateImages = oReadWriteData.GetContendTemplateImagesByType(type, true);

      folderName = (new ImagesHelper()).GetFolderName(folderName, type);

      var pathReading = Path.Combine(Directory.GetCurrentDirectory(), folderName);

      if (Directory.Exists(pathReading) && arlIContentTemplateImages != null)
      {
        string[] fileEntries = Directory.GetFiles(Path.Combine(pathReading, "original"));
        foreach (string fileName in fileEntries)
        {
          FileInfo oFileInfo = new FileInfo(fileName);
          CImages img = new CImages();
          img.index = index;
          img.urlOriginal = _oSettings.UrlImagePath + "images/" + type.ToString() + "/original/" + oFileInfo.Name;
          img.urlLarge = _oSettings.UrlImagePath + "images/" + type.ToString() + "/large/" + oFileInfo.Name;
          img.urlSmall = _oSettings.UrlImagePath + "images/" + type.ToString() + "/small/" + oFileInfo.Name;
          img.urlThumb = _oSettings.UrlImagePath + "images/" + type.ToString() + "/thumb/" + oFileInfo.Name;

          img.title = oFileInfo.Name;
          img.subTitle = string.Empty;
          img.description = string.Empty;
          img.type = type;
          img.imageName = oFileInfo.Name;

          arlImages.Add(img);
          index = index + 1;
        }
      }

      return arlImages;
    }


    /// <summary>
    /// Alle Bilder von C:\\Work\\app\\app_ahnenforschung\\app_ahnenforschung\\UploadFiles\\PersonImageMapping" kopieren.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = AppUserRoleAdmin)]
    [HttpDelete("doResizeImages/{id}")]
    public IActionResult doResizeImages(int id)
    {

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var httpPath = _oSettings.UrlImagePath + "/images/imagePerson";
      var sourcefolderName = "C:\\Work\\app\\app_ahnenforschung\\app_ahnenforschung\\UploadFiles\\PersonImageMapping";// Path.Combine("UploadFiles", "PersonImageMapping");
      var targetfolderNameNum = Path.Combine("resources", "images");
      targetfolderNameNum = Path.Combine(targetfolderNameNum, "imagePerson");
      targetfolderNameNum = Path.Combine(targetfolderNameNum, "imagePersonNum");
      var pathToSaveNum = Path.Combine(Directory.GetCurrentDirectory(), targetfolderNameNum);

      string strImagePathOriginalNum = Path.Combine(pathToSaveNum, CImages.EFileDirectory.original.ToString());
      string strImagePathLargeNum = Path.Combine(pathToSaveNum, CImages.EFileDirectory.large.ToString());
      string strImageFileSmallNum = Path.Combine(pathToSaveNum, CImages.EFileDirectory.small.ToString());
      string strImagePathThumbNum = Path.Combine(pathToSaveNum, CImages.EFileDirectory.thumb.ToString());

      var targetfolderNameRow = Path.Combine("resources", "images");
      targetfolderNameRow = Path.Combine(targetfolderNameRow, "imagePerson");
      targetfolderNameRow = Path.Combine(targetfolderNameRow, "imagePersonRow");
      var pathToSaveRow = Path.Combine(Directory.GetCurrentDirectory(), targetfolderNameRow);

      string strImagePathOriginalRow = Path.Combine(pathToSaveRow, CImages.EFileDirectory.original.ToString());
      string strImagePathLargeRow = Path.Combine(pathToSaveRow, CImages.EFileDirectory.large.ToString());
      string strImageFileSmallRow = Path.Combine(pathToSaveRow, CImages.EFileDirectory.small.ToString());
      string strImagePathThumbRow = Path.Combine(pathToSaveRow, CImages.EFileDirectory.thumb.ToString());

      CReadWriteData oReadWriteData = new CReadWriteData();

      var allFiles = Directory.GetFiles(sourcefolderName, "*.jpg", SearchOption.AllDirectories);

      foreach (string sourcePath in allFiles)
      {
        string fileName = System.IO.Path.GetFileName(sourcePath);
        // string fullPath = System.IO.Path.GetFullPath(sourcePath);
        //destFile = System.IO.Path.Combine(targetPath, fileName);
        //System.IO.File.Copy(s, destFile, true);

        if (sourcePath.ToUpper().IndexOf("_NUM", 0) > 0 || sourcePath.ToUpper().IndexOf("-NUM", 0) > 0)
        {
          System.IO.File.Copy(sourcePath, Path.Combine(strImagePathOriginalNum, fileName), true);
          (new ImagesHelper()).Resize(
              Path.Combine(strImagePathOriginalNum, fileName),
              Path.Combine(strImagePathLargeNum, fileName),
              Path.Combine(strImageFileSmallNum, fileName),
              Path.Combine(strImagePathThumbNum, fileName));

          System.IO.File.Copy(sourcePath, Path.Combine(strImagePathOriginalNum, fileName), true);
        }
        else if (sourcePath.ToUpper().IndexOf("_ROW", 0) > 0 || sourcePath.ToUpper().IndexOf("-ROW", 0) > 0)
        {
          System.IO.File.Copy(sourcePath, Path.Combine(strImagePathOriginalRow, fileName), true);
          (new ImagesHelper()).Resize(
              Path.Combine(strImagePathOriginalRow, fileName),
              Path.Combine(strImagePathLargeRow, fileName),
              Path.Combine(strImageFileSmallRow, fileName),
              Path.Combine(strImagePathThumbRow, fileName));
        }
        else
        {
          System.IO.File.Copy(sourcePath, Path.Combine(strImagePathOriginalRow, fileName), true);
          System.IO.File.Copy(sourcePath, Path.Combine(strImagePathOriginalNum, fileName), true);

          (new ImagesHelper()).Resize(
              Path.Combine(strImagePathOriginalRow, fileName),
              Path.Combine(strImagePathLargeRow, fileName),
              Path.Combine(strImageFileSmallRow, fileName),
              Path.Combine(strImagePathThumbRow, fileName));

          (new ImagesHelper()).Resize(
              Path.Combine(strImagePathOriginalNum, fileName),
              Path.Combine(strImagePathLargeNum, fileName),
              Path.Combine(strImageFileSmallNum, fileName),
              Path.Combine(strImagePathThumbNum, fileName));
        }

      }

      List<CImagePerson> imagePersons = oReadWriteData.GetAllImagePersons();
      foreach (CImagePerson imgPer in imagePersons)
      {
        imgPer.ImagePath = httpPath;
        oReadWriteData.CreateOrUpdateImagePersons(imgPer);

      }
      //    return new ImagePerson;

      if (allFiles == null)
      {
        return NotFound();
      }
      return Ok(allFiles[0]);

    }
  }
}

