using app_familyBackend.DataContext;
using appAhnenforschungData.Models.App;
using appAhnenforschungData.Models.DB;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ValueObject;
using static appAhnenforschungBackEnd.DataManager.CGlobal;

namespace app_familyChronikApi.ReadWriteDB
{
  public class ReadWirteContents(MyDatabaseContext context)
  {
    private readonly MyDatabaseContext _context = context;

    #region  get 
    public async Task<IEnumerable<ValueObject.ContentTemplate>> GetContentTemplates()
    {
      List<ValueObject.ContentTemplate> contentTemplates = [];
      var entitys = await _context.ContentTemplates.ToListAsync();

      foreach (var entity in entitys)
      {
        //var entityLinkss = await GetSingleContentTemplateLink _context.ContentTemplates.ToListAsync();
        var obj = new ValueObject.ContentTemplate();
        this.MapEntityToContentTemplate(entity, obj);

        obj.ContentTemplateLinks = (List<ValueObject.ContentTemplateLink>)await GetContentTemplateLinks(entity.Id);
        obj.ContentTemplateImages = (List<ValueObject.ContentTemplateImage>)await GetContentTemplateImageByContentTemplateId(entity.Id);
        contentTemplates.Add(obj);
      }
      return contentTemplates;
    }

    public async Task<ValueObject.ContentTemplate> GetSingleContentTemplate(Guid id)
    {


      var entity = await _context.ContentTemplates.FirstOrDefaultAsync(x => x.Id == id);

      var obj = new ValueObject.ContentTemplate();
      obj.ContentTemplateLinks = (List<ValueObject.ContentTemplateLink>)await GetContentTemplateLinks(entity.Id);
      obj.ContentTemplateImages = (List<ValueObject.ContentTemplateImage>)await GetContentTemplateImageByContentTemplateId(entity.Id);
      this.MapEntityToContentTemplate(entity, obj);

      return obj;
    }

    public async Task<ValueObject.ContentTemplateImage> GetSingleContentTemplateImage(Guid id)
    {
      var entity = await _context.ContentTemplateImages.FirstOrDefaultAsync(x => x.Id == id);

      var obj = new ValueObject.ContentTemplateImage();
      this.MapEntityToContentTemplateImage(entity, obj);

      return obj;
    }

    public async Task<IEnumerable<ValueObject.ContentTemplate>> GetContendTemplatesByType(int type)
    {
      try
      {

        List<ValueObject.ContentTemplate> templates = new List<ValueObject.ContentTemplate>();
        foreach (var entity in _context.ContentTemplates.Where(x => x.Type == (TemplateTypes)type))
        {
          var template = new ValueObject.ContentTemplate();
          this.MapEntityToContentTemplate(entity, template);

          foreach (var entityLink in _context.ContentTemplateLinks.Where(x => x.ContentTemplateId == entity.Id))
          {
            var link = new ValueObject.ContentTemplateLink();
            this.MapEntityToContentTemplateLink(entityLink, link);
            template.ContentTemplateLinks.Add(link);
          }

          foreach (var entityImage in _context.ContentTemplateImages.Where(x => x.ContentTemplateId == entity.Id))
          {
            var image = new ValueObject.ContentTemplateImage();
            this.MapEntityToContentTemplateImage(entityImage, image);
            template.ContentTemplateImages.Add(image);
          }
          templates.Add(template);
        }

        return templates;
      }
      catch (Exception)
      {
        throw;
      }
    }


    public async Task<IEnumerable<ValueObject.ContentTemplateLink>> GetContentTemplateLinks(Guid id)
    {

      List<ValueObject.ContentTemplateLink> contentTemplateLinks = [];
      var entitys = await _context.ContentTemplateLinks.Where(x=>x.ContentTemplateId == id).ToListAsync();

      foreach (var entity in entitys)
      {
        var obj = new ValueObject.ContentTemplateLink();
        this.MapEntityToContentTemplateLink(entity, obj);
        contentTemplateLinks.Add(obj);
      }
      return contentTemplateLinks;
    }

    public async Task<IEnumerable<ValueObject.ContentTemplateImage>> GetContentTemplateImages(Guid id)
    {

      List<ValueObject.ContentTemplateImage> contentTemplateImages = [];
      var entitys = await _context.ContentTemplateImages.Where(x => x.ContentTemplateId == id).ToListAsync();

      foreach (var entity in entitys)
      {
        var obj = new ValueObject.ContentTemplateImage();
        this.MapEntityToContentTemplateImage(entity, obj);
        contentTemplateImages.Add(obj);
      }
      return contentTemplateImages;
    }


    public async Task<IList<ValueObject.ContentTemplateImage>> GetContendTemplateImagesByType(TemplateTypes type, bool onlyActice)
    {
      try
      {
        List<ValueObject.ContentTemplateImage> arlImages = new List<ValueObject.ContentTemplateImage>();

        if (onlyActice)
        {
          var entity = await _context.ContentTemplates.FirstOrDefaultAsync(x => x.Type == type && x.Active == onlyActice);
          if (entity != null)
          {
            foreach (Entity.ContentTemplateImage tableImage in _context.ContentTemplateImages.Where(x => x.ContentTemplateId == entity.Id && x.Active == onlyActice))
            {
              var obj = new ValueObject.ContentTemplateImage();
              MapEntityToContentTemplateImage(tableImage, obj);
              arlImages.Add(obj);
            }
          }
        }
        else
        {
          var entity = await _context.ContentTemplates.FirstOrDefaultAsync(x => x.Type == type);
          if (entity != null)
          {
            foreach (Entity.ContentTemplateImage tableImage in _context.ContentTemplateImages.Where(x => x.ContentTemplateId == entity.Id))
            {
              var obj = new ValueObject.ContentTemplateImage();
              MapEntityToContentTemplateImage(tableImage, obj);
              arlImages.Add(obj);
            }
          }
        }

        return arlImages;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public async Task<IEnumerable<ValueObject.ContentTemplateImage>> GetContendTemplateImagesByType(Guid id)
    {

      List<ValueObject.ContentTemplateImage> contentTemplateImages = [];
      var entitys = await _context.ContentTemplateImages.Where(x => x.ContentTemplateId == id).ToListAsync();

      foreach (var entity in entitys)
      {
        var obj = new ValueObject.ContentTemplateImage();
        this.MapEntityToContentTemplateImage(entity, obj);
        contentTemplateImages.Add(obj);
      }
      return contentTemplateImages;
    }

    public async Task<ValueObject.ContentTemplateLink> GetSingleContentTemplateLink(Guid id)
    {
      var entity = await _context.ContentTemplateLinks.FirstOrDefaultAsync(x => x.Id == id);

      var entitys = await GetMediaLibraryDocumentsByContentTemplateLinkId(entity.Id);

      var obj = new ValueObject.ContentTemplateLink();
      this.MapEntityToContentTemplateLink(entity, obj);
      obj.MediaLibraryDocuments = (List<ValueObject.MediaLibraryDocument>)entitys;

      //var link = new ValueObject.ContentTemplateLink
      //{
      //  Id = entity.Id,
      //  ContentTemplateId = entity.ContentTemplateId,
      //  Title = entity.Title,
      //  SubTitle = entity.SubTitle,
      //  IsExternalLink = entity.IsExternalLink,
      //  NavigationTo = entity.NavigationTo,
      //  PersonId = entity.PersonId,
      //  Description = entity.Description,
      //  SortNo = entity.SortNo,
      //  Active = entity.Active,
      //  MediaLibraryDocuments = (List<ValueObject.MediaLibraryDocument>)entitys
      //};

      return obj;
    }

    public async Task<ValueObject.ContentTemplateLink> GetEmptyContentTemplateLink(Guid id)
    {
      var entity = await _context.ContentTemplateLinks.FirstOrDefaultAsync(x => x.Id == id);

      var entitys = await GetMediaLibraryDocumentsByContentTemplateLinkId(entity.Id);

      var obj = new ValueObject.ContentTemplateLink();
      this.MapEntityToContentTemplateLink(entity, obj);
      obj.MediaLibraryDocuments = (List<ValueObject.MediaLibraryDocument>)entitys;

      //var link = new ValueObject.ContentTemplateLink
      //{
      //  Id = entity.Id,
      //  ContentTemplateId = entity.ContentTemplateId,
      //  Title = entity.Title,
      //  SubTitle = entity.SubTitle,
      //  IsExternalLink = entity.IsExternalLink,
      //  NavigationTo = entity.NavigationTo,
      //  PersonId = entity.PersonId,
      //  Description = entity.Description,
      //  SortNo = entity.SortNo,
      //  Active = entity.Active,
      //  MediaLibraryDocuments = (List<ValueObject.MediaLibraryDocument>)entitys
      //};

      return obj;
    }

    public async Task<ValueObject.ContentTemplateImage> GetContentTemplateImage(Guid id)
    {
      var entity = await _context.ContentTemplateImages.FirstOrDefaultAsync(x => x.Id == id);

      var obj = new ValueObject.ContentTemplateImage();
      this.MapEntityToContentTemplateImage(entity, obj);

      return obj;
    }

    public async Task<IEnumerable<ValueObject.ContentTemplateImage>> GetContentTemplateImageByContentTemplateId(Guid id)
    {
      List<ValueObject.ContentTemplateImage> contentTemplateImages = [];
      var entitys = await _context.ContentTemplateImages.Where(x => x.ContentTemplateId == id).ToListAsync();

      foreach (var entity in entitys)
      {
        var obj = new ValueObject.ContentTemplateImage();
        this.MapEntityToContentTemplateImage(entity, obj);
        contentTemplateImages.Add(obj);
      }

      return contentTemplateImages;
    }


    public async Task<ValueObject.ContentTemplate> AddContentTemplate(ValueObject.ContentTemplate contentTemplate)
    {
      try
      {
        var entity = new Entity.ContentTemplate();

        this.MapContentTemplateToEntity(contentTemplate, entity);

        _context.Add(entity);

        await _context.SaveChangesAsync();

        var added = await this.GetSingleContentTemplate(entity.Id);

        return added;
      }
      catch(Exception ex) {
        Debug.WriteLine($"Error in AddContentTemplate: {ex.Message}");
        throw;
      }
    }

    
    public async Task<ValueObject.ContentTemplateLink> AddContentTemplate(ValueObject.ContentTemplateLink link)
    {
      try
      {
        var entity = new Entity.ContentTemplateLink();// s.Where(x => x.ContentTemplateId == id).ToListAsync();

        this.MapContentTemplateLinkToEntity(link, entity);

        _context.Add(entity);

        await _context.SaveChangesAsync();

        var added = await this.GetSingleContentTemplateLink(entity.Id);

        return added;
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error in AddContentTemplateLink: {ex.Message}");
        throw;
      }
    }

    public async Task<ValueObject.ContentTemplate> UpdateContentTemplate(ValueObject.ContentTemplate template)
    {
      try
      {
        var entity = await _context.ContentTemplates.FirstOrDefaultAsync(x => x.Id == template.Id);
        if (entity != null)
        {
          this.MapContentTemplateToEntity(template, entity);

          _context.Update(entity);
          await _context.SaveChangesAsync();

          var updated = await this.GetSingleContentTemplate(entity.Id);

          return updated;
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error in AddContentTemplateLink: {ex.Message}");
        throw;
      }
      return null;
    }


    

      public async Task<ValueObject.ContentTemplateImage> AddContentTemplateImage(ValueObject.ContentTemplateImage img)
    {
      try
      {
        var entity = new Entity.ContentTemplateImage();// s.Where(x => x.ContentTemplateId == id).ToListAsync();

        this.MapContentTemplateImagesToEntity(img, entity);

        _context.Add(entity);

        await _context.SaveChangesAsync();

        var added = await this.GetSingleContentTemplateImage(entity.Id);

        return added;
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error in AddContentTemplateLink: {ex.Message}");
        throw;
      }
    }
    //public async Task<ValueObject.ContentTemplate> AddContentTemplate1(ValueObject.ContentTemplate contentTemplate)
    //{

    //  var entity = new Entity.ContentTemplate();// s.Where(x => x.ContentTemplateId == id).ToListAsync();
    //  this.MapContentTemplateToEntity(contentTemplate, entity);

    //  //var added = await this.GetSingleContentTemplate(contentTemplate.Id);
    //  //return added;


    //}


    public ValueObject.MediaLibraryDocument GetEmptyDocuments()
    {
      var contentTemplates = new ValueObject.MediaLibraryDocument
      {
        Id = Guid.NewGuid(),
        Title = string.Empty,
        Description = string.Empty,
        FilePath = string.Empty,
        ContentType = string.Empty,
        ExtractedText = string.Empty,
        FormatedHtml = string.Empty,
        Active = false,
        ContentTemplateLink = new ValueObject.ContentTemplateLink()
      }; ;

      return contentTemplates;
    }


    public async Task<IEnumerable<ValueObject.MediaLibraryDocument>> GetMediaLibraryDocuments()
    {
      List<ValueObject.MediaLibraryDocument> mediaLibraryDocuments = [];
      var entitys = await _context.MediaLibraryDocuments.ToListAsync();
      foreach (var entity in entitys)
      {
        var contentTemplateLink = await GetSingleContentTemplateLink(entity.ContentTemplateLink.Id);

        var obj = new ValueObject.MediaLibraryDocument();
        this.MapEntityToMediaLibraryDocument(entity, obj);
        mediaLibraryDocuments.Add(obj);


        //contentTemplates.Add(new ValueObject.MediaLibraryDocument
        //{
        //  Id = entity.Id,
        //  Title = entity.Title,
        //  Description = entity.Description,
        //  FilePath = entity.FilePath,
        //  ContentType = entity.ContentType,
        //  ExtractedText = entity.ExtractedText,
        //  FormatedHtml = entity.FormatedHtml,
        //  Active = entity.Active,
        //  ContentTemplateLink = contentTemplateLink
        //});
      }
      return mediaLibraryDocuments;
    }

    public async Task<ValueObject.MediaLibraryDocument> GetSingleMediaLibraryDocument(Guid id)
    {
      var entity = await _context.MediaLibraryDocuments.FirstOrDefaultAsync(x => x.Id == id);

      var obj = new ValueObject.MediaLibraryDocument();
      if (entity == null) { return obj; }
      this.MapEntityToMediaLibraryDocument(entity, obj);

      return obj;
    }

    public async Task<IEnumerable<ValueObject.MediaLibraryDocument>> GetMediaLibraryDocumentsByContentTemplateLinkId(Guid contentTemplateLinkId)
    {
      List<ValueObject.MediaLibraryDocument> mediaLibraryDocuments = [];
      var entitys = await _context.MediaLibraryDocuments.Where(x => x.ContentTemplateLink.Id == contentTemplateLinkId).ToListAsync();

      foreach (var entity in entitys)
      {
        var obj = new ValueObject.MediaLibraryDocument();
        this.MapEntityToMediaLibraryDocument(entity, obj);
        mediaLibraryDocuments.Add(obj);
      }
      return mediaLibraryDocuments;
    }

    #endregion


    #region update

    public async Task<bool> UpdatetContentTemplate(ValueObject.ContentTemplate contentTemplate)
    {
      var entity = await _context.ContentTemplates.FirstOrDefaultAsync(x => x.Id == contentTemplate.Id);

      if (entity == null)
      {
        return false;
      }

      this.MapContentTemplateToEntity(contentTemplate, entity);

      _context.ContentTemplates.Update(entity);

      await _context.SaveChangesAsync();

      return true;
    }

    public async Task<bool> UpdatetContentTemplateLink(ValueObject.ContentTemplateLink contentTemplateLink)
    {
      var entity = await _context.ContentTemplateLinks.FirstOrDefaultAsync(x => x.Id == contentTemplateLink.Id);

      if (entity == null)
      {
        return false;
      }

      this.MapContentTemplateLinkToEntity(contentTemplateLink, entity);

      _context.ContentTemplateLinks.Update(entity);

      await _context.SaveChangesAsync();

      return true;
    }

    public async Task<bool> UpdatetContentTemplateImage(ValueObject.ContentTemplateImage contentTemplateImage)
    {
      var entity = await _context.ContentTemplateImages.FirstOrDefaultAsync(x => x.Id == contentTemplateImage.Id);

      if (entity == null)
      {
        return false;
      }

      this.MapContentTemplateImagesToEntity(contentTemplateImage, entity);

      _context.ContentTemplateImages.Update(entity);

      await _context.SaveChangesAsync();

      return true;
    }

    public async Task<bool> UpdatetMediaLibraryDocument(ValueObject.MediaLibraryDocument mediaLibraryDocument)
    {
      var entity = await _context.MediaLibraryDocuments.FirstOrDefaultAsync(x => x.Id == mediaLibraryDocument.Id);

      if (entity == null)
      {
        return false;
      }

      this.MapMediaLibraryDocumentToEntity(mediaLibraryDocument, entity);

      _context.MediaLibraryDocuments.Update(entity);

      await _context.SaveChangesAsync();

      return true;
    }

    public async Task<bool> UpdateContentTemplateImage(ValueObject.ContentTemplateImage contentTemplateImage)
    {
      var entity = await _context.ContentTemplateImages.FirstOrDefaultAsync(x => x.Id == contentTemplateImage.Id);

      if (entity == null)
      {
        return false;
      }

      this.MapContentTemplateImagesToEntity(contentTemplateImage, entity);

      _context.ContentTemplateImages.Update(entity);

      await _context.SaveChangesAsync();

      return true;
    }

    #endregion

    #region MapValueObjectToEntity
    private void MapContentTemplateLinkToEntity(ValueObject.ContentTemplateLink contentTemplateLink, Entity.ContentTemplateLink entity)
    {
      entity.Id = contentTemplateLink.Id;
      entity.ContentTemplateId = contentTemplateLink.ContentTemplateId;
      entity.Title = contentTemplateLink.Title ?? String.Empty;
      entity.SubTitle = contentTemplateLink.SubTitle ?? String.Empty;
      entity.IsExternalLink = contentTemplateLink.IsExternalLink ;
      entity.NavigationTo = contentTemplateLink.NavigationTo ?? String.Empty;
      entity.PersonId = contentTemplateLink.PersonId ?? Guid.Empty;
      entity.Description = contentTemplateLink.Description ?? String.Empty;
      entity.SortNo = contentTemplateLink.SortNo;
      entity.Active = contentTemplateLink.Active;
    }

    private void MapMediaLibraryDocumentToEntity(ValueObject.MediaLibraryDocument mediaLibraryDocument, Entity.MediaLibraryDocument entity)
    {
      entity.Id = mediaLibraryDocument.Id;
      entity.Title = mediaLibraryDocument.Title ?? String.Empty;
      entity.Description = mediaLibraryDocument.Description ?? String.Empty;
      entity.FilePath = mediaLibraryDocument.FilePath ?? String.Empty;
      entity.ContentType = mediaLibraryDocument.ContentType ?? String.Empty;
      entity.ExtractedText = mediaLibraryDocument.ExtractedText ?? String.Empty;
      entity.FormatedHtml = mediaLibraryDocument.FormatedHtml ?? String.Empty;
      entity.Active = mediaLibraryDocument.Active;
      if (mediaLibraryDocument.ContentTemplateLink != null)
      {
        var contentTemplateLinkEntity = new Entity.ContentTemplateLink();
        MapContentTemplateLinkToEntity(mediaLibraryDocument.ContentTemplateLink, contentTemplateLinkEntity);
        entity.ContentTemplateLink = contentTemplateLinkEntity;
      }
    }

    private void MapContentTemplateToEntity(ValueObject.ContentTemplate contentTemplate, Entity.ContentTemplate entity)
    {
      entity.Id = contentTemplate.Id;
      entity.RefContentTemplateId = contentTemplate.RefContentTemplateId?? -1;
      entity.Title = contentTemplate.Title?? String.Empty;
      entity.SubTitle = contentTemplate.SubTitle ?? String.Empty;
      entity.Content = contentTemplate.Content ?? String.Empty;
      entity.SortNo = contentTemplate.SortNo;
      entity.Type = contentTemplate.Type;
      entity.Active = contentTemplate.Active;
    }

    private void MapContentTemplateImagesToEntity(ValueObject.ContentTemplateImage contentTemplateImage, Entity.ContentTemplateImage entity)
    {
      entity.Id = contentTemplateImage.Id;
      entity.ContentTemplateId = contentTemplateImage.ContentTemplateId;
      entity.Title = contentTemplateImage.Title ?? String.Empty;
      entity.SubTitle = contentTemplateImage.SubTitle ?? String.Empty;
      entity.ImageName = contentTemplateImage.ImageName ?? String.Empty;
      entity.ImageOriginalName = contentTemplateImage.ImageOriginalName ?? String.Empty;
      entity.Description = contentTemplateImage.Description ?? String.Empty;
      entity.SortNo = contentTemplateImage.SortNo ?? 0 ;
      entity.Active = contentTemplateImage.Active;
    }
    #endregion

    #region MapEntityToValueObject
    private void MapEntityToContentTemplate(Entity.ContentTemplate entity, ValueObject.ContentTemplate contentTemplate)
    {
      contentTemplate.Id = entity.Id;
      contentTemplate.RefContentTemplateId = entity.RefContentTemplateId;
      contentTemplate.Title = entity.Title;
      contentTemplate.SubTitle = entity.SubTitle;
      contentTemplate.Content = entity.Content;
      contentTemplate.SortNo = entity.SortNo;
      contentTemplate.Type = entity.Type;
      contentTemplate.Active = entity.Active;
    }

    private void MapEntityToContentTemplateLink(Entity.ContentTemplateLink entity, ValueObject.ContentTemplateLink contentTemplateLink)
    {
      contentTemplateLink.Id = entity.Id;
      contentTemplateLink.ContentTemplateId = entity.ContentTemplateId;
      contentTemplateLink.Title = entity.Title;
      contentTemplateLink.SubTitle = entity.SubTitle;
      contentTemplateLink.IsExternalLink = entity.IsExternalLink;
      contentTemplateLink.NavigationTo = entity.NavigationTo;
      contentTemplateLink.PersonId = entity.PersonId;
      contentTemplateLink.Description = entity.Description;
      contentTemplateLink.SortNo = entity.SortNo;
      contentTemplateLink.Active = entity.Active;
    }

    public ValueObject.ContentTemplateLink EmptyContentTemplateLink()
    {
      var contentTemplateLink = new ValueObject.ContentTemplateLink
      {
        Id = Guid.NewGuid(),
        ContentTemplateId = Guid.Empty,
        Title = string.Empty,
        SubTitle = string.Empty,
        IsExternalLink = false,
        NavigationTo = string.Empty,
        PersonId = null,
        Description = string.Empty,
        SortNo = 0,
        Active = false
      };
      return contentTemplateLink;
    }

    public void MapEntityToMediaLibraryDocument(Entity.MediaLibraryDocument entity, ValueObject.MediaLibraryDocument mediaLibraryDocument)
    {
      mediaLibraryDocument.Id = entity.Id;
      mediaLibraryDocument.Title = entity.Title;
      mediaLibraryDocument.Description = entity.Description;
      mediaLibraryDocument.FilePath = entity.FilePath;
      mediaLibraryDocument.ContentType = entity.ContentType;
      mediaLibraryDocument.ExtractedText = entity.ExtractedText;
      mediaLibraryDocument.Keywords = entity.ExtractedText;
      mediaLibraryDocument.KeywordsJson = entity.KeywordsJson;
      mediaLibraryDocument.Summary = entity.Summary;
      mediaLibraryDocument.FormatedHtml = entity.FormatedHtml;
      mediaLibraryDocument.Active = entity.Active;
      if (entity.ContentTemplateLink != null)
      {
        var contentTemplateLinkVO = new ValueObject.ContentTemplateLink();
        MapEntityToContentTemplateLink(entity.ContentTemplateLink, contentTemplateLinkVO);
        mediaLibraryDocument.ContentTemplateLink = contentTemplateLinkVO;
      }
    }


    private void MapEntityToContentTemplateImage(Entity.ContentTemplateImage entity, ValueObject.ContentTemplateImage contentTemplateImage)
    {
      contentTemplateImage.Id = entity.Id;
      contentTemplateImage.ContentTemplateId = entity.ContentTemplateId;
      contentTemplateImage.Title = entity.Title;
      contentTemplateImage.SubTitle = entity.SubTitle;
      contentTemplateImage.ImageName = entity.ImageName;
      contentTemplateImage.ImageOriginalName = entity.ImageOriginalName;
      contentTemplateImage.Description = entity.Description;
      contentTemplateImage.SortNo = entity.SortNo;
      contentTemplateImage.Active = entity.Active;
    }

    #endregion

    //private void MapEntityToMediaLibraryDocumentList(List<Entity.MediaLibraryDocument> entities, List<ValueObject.MediaLibraryDocument> mediaLibraryDocuments)
    //{
    //  foreach (var entity in entities)
    //  {
    //    var mediaLibraryDocumentVO = new ValueObject.MediaLibraryDocument();
    //    MapEntityToMediaLibraryDocument(entity, mediaLibraryDocumentVO);
    //    mediaLibraryDocuments.Add(mediaLibraryDocumentVO);
    //  }
    //}

    //private void MapEntityToContentTemplateLinkList(List<Entity.ContentTemplateLink> entities, List<ValueObject.ContentTemplateLink> contentTemplateLinks)
    //{
    //  foreach (var entity in entities)
    //  {
    //    var contentTemplateLinkVO = new ValueObject.ContentTemplateLink();
    //    MapEntityToContentTemplateLink(entity, contentTemplateLinkVO);
    //    contentTemplateLinks.Add(contentTemplateLinkVO);
    //  }
    //}

    //private void MapEntityToContentTemplateList(List<Entity.ContentTemplate> entities, List<ValueObject.ContentTemplate> contentTemplates)
    //{
    //  foreach (var entity in entities)
    //  {
    //    var contentTemplateVO = new ValueObject.ContentTemplate();
    //    MapEntityToContentTemplate(entity, contentTemplateVO);
    //    contentTemplates.Add(contentTemplateVO);
    //  }
    //}

    //private void MapContentTemplateLinkListToEntity(List<ValueObject.ContentTemplateLink> contentTemplateLinks, List<Entity.ContentTemplateLink> entities)
    //{
    //  foreach (var contentTemplateLink in contentTemplateLinks)
    //  {
    //    var entity = new Entity.ContentTemplateLink();
    //    MapContentTemplateLinkToEntity(contentTemplateLink, entity);
    //    entities.Add(entity);
    //  }
    //}

    //private void MapMediaLibraryDocumentListToEntity(List<ValueObject.MediaLibraryDocument> mediaLibraryDocuments, List<Entity.MediaLibraryDocument> entities)
    //{
    //  foreach (var mediaLibraryDocument in mediaLibraryDocuments)
    //  {
    //    var entity = new Entity.MediaLibraryDocument();
    //    MapMediaLibraryDocumentToEntity(mediaLibraryDocument, entity);
    //    entities.Add(entity);
    //  }
    //}

    //private void MapContentTemplateListToEntity(List<ValueObject.ContentTemplate> contentTemplates, List<Entity.ContentTemplate> entities)
    //{
    //  foreach (var contentTemplate in contentTemplates)
    //  {
    //    var entity = new Entity.ContentTemplate();
    //    MapContentTemplateToEntity(contentTemplate, entity);
    //    entities.Add(entity);
    //  }
    //}

    //private void MapContentTemplateLinkToMediaLibraryDocument(ValueObject.ContentTemplateLink contentTemplateLink, Entity.MediaLibraryDocument entity)
    //{
    //  if (contentTemplateLink != null)
    //  {
    //    var contentTemplateLinkEntity = new Entity.ContentTemplateLink();
    //    MapContentTemplateLinkToEntity(contentTemplateLink, contentTemplateLinkEntity);
    //    entity.ContentTemplateLink = contentTemplateLinkEntity;
    //  }
    //}

    //private void MapContentTemplateLinkToMediaLibraryDocumentList(ValueObject.ContentTemplateLink contentTemplateLink, List<Entity.MediaLibraryDocument> entities)
    //{
    //  if (contentTemplateLink != null)
    //  {
    //    var contentTemplateLinkEntity = new Entity.ContentTemplateLink();
    //    MapContentTemplateLinkToEntity(contentTemplateLink, contentTemplateLinkEntity);
    //    foreach (var entity in entities)
    //    {
    //      entity.ContentTemplateLink = contentTemplateLinkEntity;
    //    }
    //  }
    //}

    //private void MapContentTemplateLinkListToMediaLibraryDocumentList(List<ValueObject.ContentTemplateLink> contentTemplateLinks, List<Entity.MediaLibraryDocument> entities)
    //{
    //  if (contentTemplateLinks != null && contentTemplateLinks.Count > 0)
    //  {
    //    foreach (var contentTemplateLink in contentTemplateLinks)
    //    {
    //      var contentTemplateLinkEntity = new Entity.ContentTemplateLink();
    //      MapContentTemplateLinkToEntity(contentTemplateLink, contentTemplateLinkEntity);
    //      foreach (var entity in entities)
    //      {
    //        entity.ContentTemplateLink = contentTemplateLinkEntity;
    //      }
    //    }
    //  }
    //}

    //private void MapContentTemplateLinkListToMediaLibraryDocument(ValueObject.ContentTemplateLink contentTemplateLink, List<Entity.MediaLibraryDocument> entities)
    //{
    //  if (contentTemplateLink != null)
    //  {
    //    var contentTemplateLinkEntity = new Entity.ContentTemplateLink();
    //    MapContentTemplateLinkToEntity(contentTemplateLink, contentTemplateLinkEntity);
    //    foreach (var entity in entities)
    //    {
    //      entity.ContentTemplateLink = contentTemplateLinkEntity;
    //    }
    //  }
    //}

    //private void MapContentTemplateLinkToContentTemplate(ValueObject.ContentTemplateLink contentTemplateLink, Entity.ContentTemplate entity)
    //{
    //  if (contentTemplateLink != null)
    //  {
    //    var contentTemplateLinkEntity = new Entity.ContentTemplateLink();
    //    MapContentTemplateLinkToEntity(contentTemplateLink, contentTemplateLinkEntity);
    //    entity.ContentTemplateLinks.Add(contentTemplateLinkEntity);
    //  }
    //}
  }


}