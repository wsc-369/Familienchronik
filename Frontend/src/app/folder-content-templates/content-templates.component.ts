import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { ContentTemplate, ContentTemplateImage, ContentTemplateLink, MediaLibraryDocument, TemplateTypes, translateTemplateType } from '../api/api-interfaces';
import { ContentTemplateService } from '../folder-services/content-template.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-content-templates',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './content-templates.component.html',
  styleUrls: ['./content-templates.component.css']
})
export class ContentTemplatesComponent implements OnInit, OnDestroy {

  readonly templateTypeOptions = Object.entries(TemplateTypes)
    .filter(([, v]) => typeof v === 'number' && (v as number) !== TemplateTypes.undefined)
    .map(([, value]) => ({ label: translateTemplateType(value as TemplateTypes), value: value as number }));

  private readonly emptyId = '00000000-0000-0000-0000-000000000000';
  private readonly imageBaseUrl = environment.apiUrl.replace('/api', '');

  contentTemplates: ContentTemplate[] = [];
  selectedTemplate: ContentTemplate = this.createEmptyTemplate();
  isLoading = false;
  isSaving = false;
  errorMessage = '';
  successMessage = '';
  uploadingDocumentId: string | null = null;
  uploadingImageId: string | null = null;
  imagePreviewUrls: Record<string, string> = {};
  pendingDocumentFiles: Record<string, File> = {};
  pendingImageFiles: Record<string, File> = {};

  constructor(private readonly contentTemplateService: ContentTemplateService) { }

  ngOnInit(): void {
    this.loadContentTemplates();
  }

  ngOnDestroy(): void {
    Object.values(this.imagePreviewUrls).forEach((url) => URL.revokeObjectURL(url));
  }

  onImageFileSelected(imageIndex: number, event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    const image = this.selectedTemplate.contentTemplateImages[imageIndex];
    if (!image) {
      return;
    }

    const localUrl = URL.createObjectURL(file);
    if (this.imagePreviewUrls[image.id]) {
      URL.revokeObjectURL(this.imagePreviewUrls[image.id]);
    }
    this.imagePreviewUrls[image.id] = localUrl;

    this.pendingImageFiles[image.id] = file;
  }

  uploadSelectedImage(imageIndex: number): void {
    const image = this.selectedTemplate.contentTemplateImages[imageIndex];
    if (!image) {
      return;
    }

    const file = this.pendingImageFiles[image.id];
    if (!file) {
      return;
    }

    this.uploadingImageId = image.id;
    this.errorMessage = '';
    this.successMessage = '';

    this.contentTemplateService.uploadContentTemplateImage({
      id: image.id,
      file,
      meta: {
        contentTemplateId: image.contentTemplateId,
        title: image.title ?? '',
        subTitle: image.subTitle ?? '',
        imageName: image.imageName ?? '',
        imageOriginalName: image.imageOriginalName ?? '',
        description: image.description ?? '',
        sortNo: image.sortNo ?? 0,
        active: !!image.active
      }
    })
      .pipe(finalize(() => { this.uploadingImageId = null; }))
      .subscribe({
        next: (result) => {
          image.imageName = result.imageName;
          image.imageOriginalName = file.name;
          delete this.pendingImageFiles[image.id];
          this.successMessage = 'Bild wurde hochgeladen.';
        },
        error: (error) => {
          this.errorMessage = this.buildErrorMessage('Bild konnte nicht hochgeladen werden.', error);
        }
      });
  }

  getImagePreviewUrl(image: ContentTemplateImage): string | null {
    return this.imagePreviewUrls[image.id]
      ?? (image.imageName ? `${this.imageBaseUrl}/images/${image.imageName}` : null);
  }

  onDocumentFileSelected(linkIndex: number, documentIndex: number, event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    const link = this.selectedTemplate.contentTemplateLinks[linkIndex];
    const document = link?.mediaLibraryDocuments?.[documentIndex];
    if (!document) {
      return;
    }

    this.pendingDocumentFiles[document.id] = file;

    const fileName = file.name?.trim() ?? '';
    const fileNameWithoutExtension = fileName.replace(/\.[^/.]+$/, '');

    document.filePath = fileName || document.filePath;
    document.contentType = file.type || document.contentType;
    document.title = document.title?.trim() ? document.title : (fileNameWithoutExtension || fileName);
    document.keywords = document.keywords?.trim() ? document.keywords : (fileNameWithoutExtension || fileName);
  }

  uploadSelectedDocument(linkIndex: number, documentIndex: number): void {
    const link = this.selectedTemplate.contentTemplateLinks[linkIndex];
    const document = link?.mediaLibraryDocuments?.[documentIndex];
    if (!document) {
      return;
    }

    const file = this.pendingDocumentFiles[document.id];
    if (!file) {
      return;
    }

    this.uploadingDocumentId = document.id;
    this.errorMessage = '';
    this.successMessage = '';

    this.contentTemplateService.uploadContentTemplateDocument({
      id: document.id,
      file,
      meta: {
        contentTemplateLinkId: link?.id ?? '',
        contentTemplateId: link?.contentTemplateId ?? '',
        title: document.title ?? '',
        description: document.description ?? '',
        filePath: document.filePath ?? '',
        contentType: document.contentType ?? '',
        keywords: document.keywords ?? '',
        keywordsJson: document.keywordsJson ?? '',
        summary: document.summary ?? '',
        extractedText: document.extractedText ?? '',
        formatedHtml: document.formatedHtml ?? '',
        active: !!document.active
      }
    })
      .pipe(finalize(() => {
        this.uploadingDocumentId = null;
      }))
      .subscribe({
        next: (result) => {
          document.filePath = result.filePath?.trim() || file.name;
          document.contentType = result.contentType?.trim() || file.type || document.contentType;
          document.title = result.title?.trim() || document.title || file.name;
          document.keywords = document.keywords || file.name;
          delete this.pendingDocumentFiles[document.id];
          this.successMessage = 'Dokument wurde hochgeladen.';
        },
        error: (error) => {
          this.errorMessage = this.buildErrorMessage('Dokument konnte nicht hochgeladen werden.', error);
        }
      });
  }

  getPendingDocumentFileName(document: MediaLibraryDocument): string {
    return this.pendingDocumentFiles[document.id]?.name ?? '';
  }

  getPendingImageFileName(image: ContentTemplateImage): string {
    return this.pendingImageFiles[image.id]?.name ?? '';
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
    const link = this.selectedTemplate.contentTemplateLinks[index];
    if (!link) {
      return;
    }

    const shouldDeleteInDb = this.isPersistedLink(link.id);
    if (!shouldDeleteInDb) {
      this.selectedTemplate.contentTemplateLinks.splice(index, 1);
      return;
    }

    this.successMessage = '';
    this.errorMessage = '';
    this.isSaving = true;

    this.contentTemplateService.deleteContentTemplateLink(link.id)
      .pipe(finalize(() => {
        this.isSaving = false;
      }))
      .subscribe({
        next: () => {
          this.selectedTemplate.contentTemplateLinks.splice(index, 1);
          this.removePersistedLinkFromCache(link.id);
          this.successMessage = 'Link wurde geloescht.';
        },
        error: (error) => {
          this.errorMessage = this.buildErrorMessage('Link konnte nicht geloescht werden.', error);
        }
      });
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

    const document = link.mediaLibraryDocuments[documentIndex];
    if (!document) {
      return;
    }

    const shouldDeleteInDb = this.isPersistedDocument(document.id);
    if (!shouldDeleteInDb) {
      link.mediaLibraryDocuments.splice(documentIndex, 1);
      return;
    }

    this.successMessage = '';
    this.errorMessage = '';
    this.isSaving = true;

    this.contentTemplateService.deleteContentMediaLibraryDocument(document.id)
      .pipe(finalize(() => {
        this.isSaving = false;
      }))
      .subscribe({
        next: () => {
          link.mediaLibraryDocuments.splice(documentIndex, 1);
          this.removePersistedDocumentFromCache(document.id);
          this.successMessage = 'Dokument wurde geloescht.';
        },
        error: (error) => {
          this.errorMessage = this.buildErrorMessage('Dokument konnte nicht geloescht werden.', error);
        }
      });
  }

  addImage(): void {
    this.selectedTemplate.contentTemplateImages.push(this.createEmptyImage(this.selectedTemplate.id));
  }

  removeImage(index: number): void {
    const image = this.selectedTemplate.contentTemplateImages[index];
    if (!image) {
      return;
    }

    const shouldDeleteInDb = this.isPersistedImage(image.id);
    if (!shouldDeleteInDb) {
      this.selectedTemplate.contentTemplateImages.splice(index, 1);
      return;
    }

    this.successMessage = '';
    this.errorMessage = '';
    this.isSaving = true;

    this.contentTemplateService.deleteContentTemplateImage(image.id)
      .pipe(finalize(() => {
        this.isSaving = false;
      }))
      .subscribe({
        next: () => {
          this.selectedTemplate.contentTemplateImages.splice(index, 1);
          this.removePersistedImageFromCache(image.id);
          this.successMessage = 'Bild wurde geloescht.';
        },
        error: (error) => {
          this.errorMessage = this.buildErrorMessage('Bild konnte nicht geloescht werden.', error);
        }
      });
  }

  onSaveTemplate(): void {
    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEmptyId(this.selectedTemplate.id)) {
      const payload = this.normalizeTemplate(this.selectedTemplate, this.createClientGuid());
      this.contentTemplateService.createContentTemplate(payload)
        .pipe(finalize(() => {
          this.isSaving = false;
        }))
        .subscribe({
          next: (savedTemplate) => {
            this.successMessage = 'ContentTemplate wurde erstellt.';
            this.selectedTemplate = this.cloneTemplate(savedTemplate);
            this.loadContentTemplates(savedTemplate.id);
          },
          error: (error) => {
            this.errorMessage = this.buildErrorMessage('ContentTemplate konnte nicht erstellt werden.', error);
          }
        });
      return;
    }

    const payload = this.normalizeTemplate(this.selectedTemplate);
    const request$ = this.contentTemplateService.updateContentTemplate(payload.id, payload);

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
        error: (error) => {
          this.errorMessage = this.buildErrorMessage('ContentTemplate konnte nicht gespeichert werden.', error);
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
        error: (error) => {
          this.errorMessage = this.buildErrorMessage('ContentTemplate konnte nicht geloescht werden.', error);
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
        error: (error) => {
          this.errorMessage = this.buildErrorMessage('ContentTemplates konnten nicht geladen werden.', error);
        }
      });
  }

  private createEmptyTemplate(): ContentTemplate {
    return {
      id: this.emptyId,
      refContentTemplateId: undefined,
      title: '',
      subTitle: '',
      content: '',
      sortNo: 0,
      type: null,
      active: true,
      contentTemplateLinks: [],
      contentTemplateImages: [],
    };
  }

  private createEmptyLink(contentTemplateId: string): ContentTemplateLink {
    return {
      id: this.createClientGuid(),
      contentTemplateId,
      title: '--',
      subTitle: '--',
      isExternalLink: false,
      navigationTo: '',
      personId: null,
      description: '',
      sortNo: 0,
      active: true,
      mediaLibraryDocuments: []
    };
  }

  private createEmptyImage(contentTemplateId: string): ContentTemplateImage {
    return {
      id: this.createClientGuid(),
      contentTemplateId,
      title: '--',
      subTitle: '--',
      imageName: '',
      imageOriginalName: '',
      description: '',
      sortNo: 0,
      active: true
    };
  }

  private createEmptyMediaLibraryDocument(contentTemplateId: string): MediaLibraryDocument {
    return {
      id: this.createClientGuid(),
      title: '--',
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

  private normalizeTemplate(template: ContentTemplate, templateIdOverride?: string): ContentTemplate {
    const normalizedTemplateId = this.normalizeRootId(templateIdOverride ?? template.id);

    return {
      ...template,
      id: normalizedTemplateId,
      title: template.title?.trim(),
      subTitle: template.subTitle?.trim(),
      content: template.content?.trim(),
      contentTemplateLinks: (template.contentTemplateLinks ?? []).map((link) => ({
        ...link,
        id: this.normalizeChildId(link.id),
        contentTemplateId: normalizedTemplateId,
        title: link.title?.trim(),
        subTitle: link.subTitle?.trim(),
        navigationTo: link.navigationTo?.trim(),
        personId: this.normalizeOptionalGuid(link.personId),
        description: link.description?.trim(),
        mediaLibraryDocuments: (link.mediaLibraryDocuments ?? []).map((document) => ({
          ...document,
          id: this.normalizeChildId(document.id),
          title: document.title?.trim() ?? '',
          description: document.description?.trim() ?? '',
          filePath: document.filePath?.trim() ?? '',
          contentType: document.contentType?.trim() ?? '',
          extractedText: document.extractedText?.trim() ?? '',
          keywords: document.keywords?.trim() ?? '',
          keywordsJson: document.keywordsJson?.trim() ?? '',
          summary: document.summary?.trim() ?? '',
          formatedHtml: document.formatedHtml?.trim() ?? '',
          contentTemplateLink: this.normalizeDocumentLink(document.contentTemplateLink, normalizedTemplateId)
        }))
      })),
      contentTemplateImages: (template.contentTemplateImages ?? []).map((image) => ({
        ...image,
        id: this.normalizeChildId(image.id),
        contentTemplateId: normalizedTemplateId,
        title: image.title?.trim(),
        subTitle: image.subTitle?.trim(),
        imageName: image.imageName?.trim(),
        imageOriginalName: image.imageOriginalName?.trim(),
        description: image.description?.trim()
      }))
    };
  }

  private normalizeRootId(id?: string): string {
    const normalizedId = id?.trim() ?? '';
    return this.isEmptyId(normalizedId) ? '' : normalizedId;
  }

  private normalizeChildId(id?: string): string {
    const normalizedId = id?.trim() ?? '';
    return this.isEmptyId(normalizedId) ? this.createClientGuid() : normalizedId;
  }

  private normalizeOptionalGuid(id?: string | null): string | null {
    const normalizedId = id?.trim() ?? '';
    return this.isEmptyId(normalizedId) ? null : normalizedId;
  }

  private normalizeDocumentLink(link: ContentTemplateLink | undefined, contentTemplateId: string): ContentTemplateLink {
    const sourceLink = link ?? this.createEmptyLink(contentTemplateId);

    return {
      ...sourceLink,
      id: this.normalizeChildId(sourceLink.id),
      contentTemplateId,
      title: sourceLink.title?.trim() ?? '',
      subTitle: sourceLink.subTitle?.trim() ?? '',
      navigationTo: sourceLink.navigationTo?.trim() ?? '',
      personId: this.normalizeOptionalGuid(sourceLink.personId),
      description: sourceLink.description?.trim() ?? '',
      mediaLibraryDocuments: []
    };
  }

  private isEmptyId(id?: string): boolean {
    const normalizedId = id?.trim() ?? '';
    return !normalizedId || normalizedId === this.emptyId;
  }

  private createClientGuid(): string {
    const cryptoApi = globalThis.crypto;
    if (cryptoApi?.randomUUID) {
      return cryptoApi.randomUUID();
    }

    // Fallback for environments without randomUUID support.
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (char) => {
      const random = Math.floor(Math.random() * 16);
      const value = char === 'x' ? random : (random & 0x3) | 0x8;
      return value.toString(16);
    });
  }

  private buildErrorMessage(defaultMessage: string, error: unknown): string {
    const detail = this.extractErrorDetail(error);
    return detail ? `${defaultMessage} ${detail}` : defaultMessage;
  }

  private extractErrorDetail(error: unknown): string {
    if (!(error instanceof HttpErrorResponse)) {
      return '';
    }

    const payload = error.error as
      | string
      | { message?: string; title?: string; detail?: string; errors?: Record<string, string[] | string> }
      | null
      | undefined;

    if (typeof payload === 'string') {
      return payload.trim();
    }

    if (payload?.message) {
      return payload.message;
    }

    if (payload?.errors) {
      const flattened = Object.entries(payload.errors)
        .flatMap(([field, entry]) => {
          const messages = Array.isArray(entry) ? entry : [entry];
          return messages.map((message) => `${field}: ${`${message}`.trim()}`);
        })
        .filter((entry) => !!entry);

      if (flattened.length > 0) {
        return flattened.join(' | ');
      }
    }

    if (payload?.detail) {
      return payload.detail;
    }

    if (payload?.title) {
      return payload.title;
    }

    if (error.status) {
      return `HTTP ${error.status}`;
    }

    return '';
  }

  private getPersistedSelectedTemplate(): ContentTemplate | undefined {
    return this.contentTemplates.find((item) => item.id === this.selectedTemplate.id);
  }

  private isPersistedLink(id?: string): boolean {
    if (this.isEmptyId(id) || this.isEmptyId(this.selectedTemplate.id)) {
      return false;
    }

    const persistedTemplate = this.getPersistedSelectedTemplate();
    if (!persistedTemplate) {
      return true;
    }

    return (persistedTemplate.contentTemplateLinks ?? []).some((link) => link.id === id);
  }

  private isPersistedImage(id?: string): boolean {
    if (this.isEmptyId(id) || this.isEmptyId(this.selectedTemplate.id)) {
      return false;
    }

    const persistedTemplate = this.getPersistedSelectedTemplate();
    if (!persistedTemplate) {
      return true;
    }

    return (persistedTemplate.contentTemplateImages ?? []).some((image) => image.id === id);
  }

  private isPersistedDocument(id?: string): boolean {
    if (this.isEmptyId(id) || this.isEmptyId(this.selectedTemplate.id)) {
      return false;
    }

    const persistedTemplate = this.getPersistedSelectedTemplate();
    if (!persistedTemplate) {
      return true;
    }

    return (persistedTemplate.contentTemplateLinks ?? []).some((link) =>
      (link.mediaLibraryDocuments ?? []).some((document) => document.id === id)
    );
  }

  private removePersistedLinkFromCache(id: string): void {
    const persistedTemplate = this.getPersistedSelectedTemplate();
    if (!persistedTemplate) {
      return;
    }

    persistedTemplate.contentTemplateLinks = (persistedTemplate.contentTemplateLinks ?? []).filter((link) => link.id !== id);
  }

  private removePersistedImageFromCache(id: string): void {
    const persistedTemplate = this.getPersistedSelectedTemplate();
    if (!persistedTemplate) {
      return;
    }

    persistedTemplate.contentTemplateImages = (persistedTemplate.contentTemplateImages ?? []).filter((image) => image.id !== id);
  }

  private removePersistedDocumentFromCache(id: string): void {
    const persistedTemplate = this.getPersistedSelectedTemplate();
    if (!persistedTemplate) {
      return;
    }

    persistedTemplate.contentTemplateLinks = (persistedTemplate.contentTemplateLinks ?? []).map((link) => ({
      ...link,
      mediaLibraryDocuments: (link.mediaLibraryDocuments ?? []).filter((document) => document.id !== id)
    }));
  }
}
