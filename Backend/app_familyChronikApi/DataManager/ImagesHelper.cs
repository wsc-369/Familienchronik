using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace app_familyBackend.DataManager
{
    public class ImagesHelper
    {

        /// <summary>
        /// Den Dateienverzeichnisnamen zusammenbauen
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetFolderName(string folderName, CContentTemplate.ETemplateTypes type)
        {
            return folderName = Path.Combine(folderName, type.ToString());
            //switch (type)
            //{
            //  case CContentTemplate.ETemplateTypes.excusion:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.excusion.ToString());
            //      break;
            //    }
            //  case CContentTemplate.ETemplateTypes.mediatek:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.mediatek.ToString());
            //      break;
            //    }
            //  case CContentTemplate.ETemplateTypes.restaurant:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.restaurant.ToString());
            //      break;
            //    }
            //  case CContentTemplate.ETemplateTypes.shop:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.shop.ToString());
            //      break;
            //    }
            //  case CContentTemplate.ETemplateTypes.theatricalLife:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.theatricalLife.ToString());
            //      break;
            //    }
            //  case CContentTemplate.ETemplateTypes.club:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.club.ToString());
            //      break;
            //    }
            //  case CContentTemplate.ETemplateTypes.mainPageSlide:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.mainPageSlide.ToString());
            //      break;
            //    }
            //  case CContentTemplate.ETemplateTypes.whoIsInThePhoto:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.whoIsInThePhoto.ToString());
            //      break;
            //    }
            //  case CContentTemplate.ETemplateTypes.clubAhnenforschung:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.clubAhnenforschung.ToString());
            //      break;
            //    }
            //  case CContentTemplate.ETemplateTypes.stammNamen:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.stammNamen.ToString());
            //      break;
            //    }
            //  case CContentTemplate.ETemplateTypes.themaOverview:
            //    {
            //      folderName = Path.Combine(folderName, CContentTemplate.ETemplateTypes.themaOverview.ToString());
            //      break;
            //    }

            //    //default:
            //    //  {
            //    //    folderName = Path.Combine(folderName, "other");
            //    //    break;
            //    //  }

            //}
            //return folderName;
        }

        /// <summary>
        /// Die entsprechenden Verzeichnisse erstellen
        /// </summary>
        /// <param name="pathToSave"></param>
        /// <param name="strImagePathOriginal"></param>
        /// <param name="strImagePathLarge"></param>
        /// <param name="strImageFileSmall"></param>
        /// <param name="strImagePathThumb"></param>
        public void CreateDirectories(string pathToSave, string strImagePathOriginal, string strImagePathLarge, string strImageFileSmall, string strImagePathThumb)
        {
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            if (!Directory.Exists(strImagePathOriginal))
            {
                Directory.CreateDirectory(strImagePathOriginal);
            }
            if (!Directory.Exists(strImagePathLarge))
            {
                Directory.CreateDirectory(strImagePathLarge);
            }
            if (!Directory.Exists(strImageFileSmall))
            {
                Directory.CreateDirectory(strImageFileSmall);
            }
            if (!Directory.Exists(strImagePathThumb))
            {
                Directory.CreateDirectory(strImagePathThumb);
            }

        }

        /// <summary>
        /// Für alle Bilder, Ausser der Personenbilder werden hier die Bilder in den weiteren Grössen erstellt.
        /// </summary>
        /// <param name="strImagePathOriginal"></param>
        /// <param name="strImagePathLarge"></param>
        /// <param name="strImageFileSmall"></param>
        /// <param name="strImagePathThumb"></param>
        public void Resize(string strImagePathOriginal, string strImagePathLarge, string strImageFileSmall, string strImagePathThumb)
        {
            Image imageOriginal; // https://docs.sixlabors.com/articles/imagesharp/resize.html
            for (int i = 1; i < 4; i++)
            {
                switch (i)
                {
                    case 1:
                        imageOriginal = Image.Load(strImagePathOriginal);
                        if (imageOriginal.Width > imageOriginal.Height)
                        {
                            imageOriginal.Mutate(ctx => ctx.Resize(CGlobal.ImageLargeSize().Key, CGlobal.ImageLargeSize().Value)); ;
                        }
                        else
                        {
                            imageOriginal.Mutate(ctx => ctx.Resize(CGlobal.ImageLargeSize().Value, CGlobal.ImageLargeSize().Key));
                        }

                        imageOriginal.Save(strImagePathLarge); // based on the file extension pick an encoder then encode and write the data to disk
                        imageOriginal.Dispose();
                        break;
                    case 2:
                        imageOriginal = Image.Load(strImagePathOriginal);
                        if (imageOriginal.Width > imageOriginal.Height)
                        {
                            imageOriginal.Mutate(ctx => ctx.Resize(CGlobal.ImageSmallSize().Key, CGlobal.ImageSmallSize().Value));
                        }
                        else
                        {
                            imageOriginal.Mutate(ctx => ctx.Resize(CGlobal.ImageSmallSize().Value, CGlobal.ImageSmallSize().Key));
                        }
                        imageOriginal.Save(strImageFileSmall);
                        imageOriginal.Dispose();
                        break;
                    case 3:
                        imageOriginal = Image.Load(strImagePathOriginal);
                        if (imageOriginal.Width > imageOriginal.Height)
                        {
                            imageOriginal.Mutate(ctx => ctx.Resize(CGlobal.ImageThumbSize().Key, CGlobal.ImageThumbSize().Value));
                        }
                        else
                        {
                            imageOriginal.Mutate(ctx => ctx.Resize(CGlobal.ImageThumbSize().Value, CGlobal.ImageThumbSize().Key));
                        }

                        imageOriginal.Save(strImagePathThumb);
                        imageOriginal.Dispose();
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            }
        }

        /// <summary>
        /// Für die Personenbilder werden hier die Bilder in den weiteren Grössen erstellt.
        /// </summary>
        /// <param name="strImagePathOriginal"></param>
        /// <param name="strImagePathLarge"></param>
        /// <param name="strImageFileSmall"></param>
        /// <param name="strImagePathThumb"></param>
        public void ResizePersonalImage(string strImagePathOriginal, string strImagePathLarge, string strImageFileSmall, string strImagePathThumb)
        {
            Image imageOriginal; // https://docs.sixlabors.com/articles/imagesharp/resize.html
            for (int i = 1; i < 4; i++)
            {
                switch (i)
                {
                    case 1:
                        imageOriginal = Image.Load(strImagePathOriginal);
                        imageOriginal.Mutate(ctx => ctx.Resize(CGlobal.ImagePersonLargeSize().Key, CGlobal.ImagePersonLargeSize().Value)); // resize the image in place and return it for chaining
                        imageOriginal.Save(strImagePathLarge); // based on the file extension pick an encoder then encode and write the data to disk
                        imageOriginal.Dispose();
                        break;
                    case 2:
                        imageOriginal = Image.Load(strImagePathOriginal);
                        imageOriginal.Mutate(ctx => ctx.Resize(CGlobal.ImagePersonSmallSize().Key, CGlobal.ImagePersonSmallSize().Value));
                        imageOriginal.Save(strImageFileSmall);
                        imageOriginal.Dispose();
                        break;
                    case 3:
                        imageOriginal = Image.Load(strImagePathOriginal);
                        imageOriginal.Mutate(ctx => ctx.Resize(CGlobal.ImagePersonThumbSize().Key, CGlobal.ImagePersonThumbSize().Value));
                        imageOriginal.Save(strImagePathThumb);
                        imageOriginal.Dispose();
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            }
        }
    }
}
