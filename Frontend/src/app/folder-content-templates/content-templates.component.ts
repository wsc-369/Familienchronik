import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { ContentTemplate, ContentTemplateImage, ContentTemplateLink, MediaLibraryDocument } from '../api/api-interfaces';
import { ContentTemplateService } from '../folder-services/content-template.service';

@Component({
  selector: 'app-content-templates',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './content-templates.component.html',
  styleUrls: ['./content-templates.component.css']
})
export class ContentTemplatesComponent implements OnInit {
  contentTemplates: ContentTemplate[] = [];
  selectedTemplate: ContentTemplate = this.createEmptyTemplate();
  isLoading = false;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  constructor(private readonly contentTemplateService: ContentTemplateService) {}

  ngOnInit(): void {
    this.loadContentTemplates();
  }

  trackByTemplateId(_: number, item: ContentTemplate): string {
    return item.id;
  }

  trackByLinkId(index: number, item: ContentTemplateLink): string {
    return item.id || `link-${index}`;
  }

  trackByImageId(index: number, item: ContentTemplateImage): string {
    return item.id || `image-${index}`;
  }

  trackByDocumentId(index: number, item: MediaLibraryDocument): string {
    return item.id || `document-${index}`;
  }

  onCreateNew(): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.selectedTemplate = this.createEmptyTemplate();
  }

  onSelectTemplate(template: ContentTemplate): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.selectedTemplate = this.cloneTemplate(template);
  }

  addLink(): void {
    this.selectedTemplate.contentTemplateLinks.push(this.createEmptyLink(this.selectedTemplate.id));
  }

  removeLink(index: number): void {
    this.selectedTemplate.contentTemplateLinks.splice(index, 1);
  }

  addMediaLibraryDocument(linkIndex: number): void {
    const link = this.selectedTemplate.contentTemplateLinks[linkIndex];
    if (!link) {
      return;
    }

    link.mediaLibraryDocuments = link.mediaLibraryDocuments ?? [];
    link.mediaLibraryDocuments.push(this.createEmptyMediaLibraryDocument(link.contentTemplateId));
  }

  removeMediaLibraryDocument(linkIndex: number, documentIndex: number): void {
    const link = this.selectedTemplate.contentTemplateLinks[linkIndex];
    if (!link?.mediaLibraryDocuments) {
      return;
    }

    link.mediaLibraryDocuments.splice(documentIndex, 1);
  }

  addImage(): void {
    this.selectedTemplate.contentTemplateImages.push(this.createEmptyImage(this.selectedTemplate.id));
  }

  removeImage(index: number): void {
    this.selectedTemplate.contentTemplateImages.splice(index, 1);
  }

  onSaveTemplate(): void {
    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const payload = this.normalizeTemplate(this.selectedTemplate);
    const request$ = payload.id
      ? this.contentTemplateService.updateContentTemplate(payload.id, payload)
      : this.contentTemplateService.createContentTemplate(payload);

    request$
      .pipe(finalize(() => {
        this.isSaving = false;
      }))
      .subscribe({
        next: (savedTemplate) => {
          this.successMessage = 'ContentTemplate wurde gespeichert.';
          this.selectedTemplate = this.cloneTemplate(savedTemplate);
          this.loadContentTemplates(savedTemplate.id);
        },
        error: () => {
          this.errorMessage = 'ContentTemplate konnte nicht gespeichert werden.';
        }
      });
  }

  onDeleteTemplate(): void {
    if (!this.selectedTemplate.id) {
      this.onCreateNew();
      return;
    }

    const confirmed = window.confirm('Soll dieses ContentTemplate wirklich geloescht werden?');
    if (!confirmed) {
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.contentTemplateService
      .deleteContentTemplate(this.selectedTemplate.id)
      .pipe(finalize(() => {
        this.isSaving = false;
      }))
      .subscribe({
        next: () => {
          this.successMessage = 'ContentTemplate wurde geloescht.';
          this.selectedTemplate = this.createEmptyTemplate();
          this.loadContentTemplates();
        },
        error: () => {
          this.errorMessage = 'ContentTemplate konnte nicht geloescht werden.';
        }
      });
  }

  private loadContentTemplates(selectId?: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.contentTemplateService
      .getContentTemplates()
      .pipe(finalize(() => {
        this.isLoading = false;
      }))
      .subscribe({
        next: (templates) => {
          this.contentTemplates = templates ?? [];

          if (selectId) {
            const selected = this.contentTemplates.find((item) => item.id === selectId);
            if (selected) {
              this.selectedTemplate = this.cloneTemplate(selected);
            }
          }
        },
        error: () => {
          this.errorMessage = 'ContentTemplates konnten nicht geladen werden.';
        }
      });
  }

  private createEmptyTemplate(): ContentTemplate {
    return {
      id: '',
      refContentTemplateId: undefined,
      title: '',
      subTitle: '',
      content: '',
      sortNo: 0,
      type: 0,
      active: true,
      contentTemplateLinks: [],
      contentTemplateImages: []
    };
  }

  private createEmptyLink(contentTemplateId: string): ContentTemplateLink {
    return {
      id: '',
      contentTemplateId,
      title: '',
      subTitle: '',
      isExternalLink: false,
      navigationTo: '',
      personId: '',
      description: '',
      sortNo: 0,
      active: true,
      mediaLibraryDocuments: []
    };
  }

  private createEmptyImage(contentTemplateId: string): ContentTemplateImage {
    return {
      id: '',
      contentTemplateId,
      title: '',
      subTitle: '',
      imageName: '',
      imageOriginalName: '',
      description: '',
      sortNo: 0,
      active: true
    };
  }

  private createEmptyMediaLibraryDocument(contentTemplateId: string): MediaLibraryDocument {
    return {
      id: '',
      title: '',
      description: '',
      filePath: '',
      contentType: '',
      extractedText: '',
      keywords: '',
      keywordsJson: '',
      summary: '',
      formatedHtml: '',
      active: true,
      contentTemplateLink: this.createEmptyLink(contentTemplateId)
    };
  }

  private cloneTemplate(template: ContentTemplate): ContentTemplate {
    return {
      ...template,
      contentTemplateLinks: (template.contentTemplateLinks ?? []).map((link) => ({
        ...link,
        mediaLibraryDocuments: (link.mediaLibraryDocuments ?? []).map((document) => ({ ...document }))
      })),
      contentTemplateImages: (template.contentTemplateImages ?? []).map((image) => ({ ...image }))
    };
  }

  private normalizeTemplate(template: ContentTemplate): ContentTemplate {
    const normalizedTemplateId = template.id?.trim() ?? '';

    return {
      ...template,
      id: normalizedTemplateId,
      title: template.title?.trim(),
      subTitle: template.subTitle?.trim(),
      content: template.content?.trim(),
      contentTemplateLinks: (template.contentTemplateLinks ?? []).map((link) => ({
        ...link,
        id: link.id?.trim() ?? '',
        contentTemplateId: link.contentTemplateId?.trim() || normalizedTemplateId,
        title: link.title?.trim(),
        subTitle: link.subTitle?.trim(),
        navigationTo: link.navigationTo?.trim(),
        personId: link.personId?.trim(),
        description: link.description?.trim(),
        mediaLibraryDocuments: (link.mediaLibraryDocuments ?? []).map((document) => ({
          ...document,
          id: document.id?.trim() ?? '',
          title: document.title?.trim() ?? '',
          description: document.description?.trim() ?? '',
          filePath: document.filePath?.trim() ?? '',
          contentType: document.contentType?.trim() ?? '',
          extractedText: document.extractedText?.trim() ?? '',
          keywords: document.keywords?.trim() ?? '',
          keywordsJson: document.keywordsJson?.trim() ?? '',
          summary: document.summary?.trim() ?? '',
          formatedHtml: document.formatedHtml?.trim() ?? '',
          contentTemplateLink: document.contentTemplateLink ?? this.createEmptyLink(link.contentTemplateId?.trim() || normalizedTemplateId)
        }))
      })),
      contentTemplateImages: (template.contentTemplateImages ?? []).map((image) => ({
        ...image,
        id: image.id?.trim() ?? '',
        contentTemplateId: image.contentTemplateId?.trim() || normalizedTemplateId,
        title: image.title?.trim(),
        subTitle: image.subTitle?.trim(),
        imageName: image.imageName?.trim(),
        imageOriginalName: image.imageOriginalName?.trim(),
        description: image.description?.trim()
      }))
    };
  }
}
