import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
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
  private readonly emptyId = '00000000-0000-0000-0000-000000000000';

  contentTemplates: ContentTemplate[] = [];
  selectedTemplate: ContentTemplate = this.createEmptyTemplate();
  isLoading = false;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  constructor(private readonly contentTemplateService: ContentTemplateService) { }

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
      type: -1,
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
}
