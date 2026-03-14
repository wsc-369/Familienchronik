import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContentTemplate } from '../api/api-interfaces';
import { environment } from '../../environments/environment';

interface UploadDocumentResponse {
  filePath?: string;
  fileName?: string;
  contentType?: string;
  title?: string;
}

interface UploadImageMeta {
  contentTemplateId: string;
  title: string;
  subTitle: string;
  imageName: string;
  imageOriginalName: string;
  description: string;
  sortNo: number;
  active: boolean;
}

interface UploadDocumentMeta {
  contentTemplateLinkId: string;
  contentTemplateId: string;
  title: string;
  description: string;
  filePath: string;
  contentType: string;
  keywords: string;
  keywordsJson: string;
  summary: string;
  extractedText: string;
  formatedHtml: string;
  active: boolean;
}

interface UploadImagePayload {
  id: string;
  file: File;
  meta: UploadImageMeta;
}

interface UploadDocumentPayload {
  id: string;
  file: File;
  meta: UploadDocumentMeta;
}

type UploadPayloadBase = {
  id: string;
  file: File;
  meta: object;
};

@Injectable({ providedIn: 'root' })
export class ContentTemplateService {
  private readonly baseUrl = `${environment.apiUrl}/ContentTemplates`;

  constructor(private readonly http: HttpClient) { }

  getContentTemplates(): Observable<ContentTemplate[]> {
    return this.http.get<ContentTemplate[]>(this.baseUrl);
  }

  getContentTemplateById(id: string): Observable<ContentTemplate> {
    return this.http.get<ContentTemplate>(`${this.baseUrl}/${encodeURIComponent(id)}`);
  }

  getEmptyContentTemplate(): Observable<ContentTemplate> {
    return this.http.get<ContentTemplate>(`${this.baseUrl}/getEmptyContentTemplate`, {});
  }

  createContentTemplate(template: ContentTemplate): Observable<ContentTemplate> {
    return this.http.post<ContentTemplate>(this.baseUrl, template);
  }

  updateContentTemplate(id: string, template: ContentTemplate): Observable<ContentTemplate> {
    return this.http.put<ContentTemplate>(`${this.baseUrl}/updateContentTemplate/${encodeURIComponent(id)}`, template);
  }

  deleteContentTemplate(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${encodeURIComponent(id)}`);
  }

  deleteContentTemplateLink(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/deleteContentTemplateLink/${encodeURIComponent(id)}`);
  }

  deleteContentTemplateImage(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/deleteContentTemplateImage/${encodeURIComponent(id)}`);
  }

  deleteContentMediaLibraryDocument(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/deleteContentMediaLibraryDocument/${encodeURIComponent(id)}`);
  }

  uploadContentTemplateImage(payload: UploadImagePayload): Observable<{ imageName: string }> {
    const formData = this.buildUploadFormData(payload);
    return this.http.post<{ imageName: string }>(`${this.baseUrl}/uploadImage`, formData);
  }

  uploadContentTemplateDocument(payload: UploadDocumentPayload): Observable<UploadDocumentResponse> {
    const formData = this.buildUploadFormData(payload);
    return this.http.post<UploadDocumentResponse>(`${this.baseUrl}/uploadDocument`, formData);
  }

  private buildUploadFormData(payload: UploadPayloadBase): FormData {
    const formData = new FormData();
    formData.append('id', payload.id);
    formData.append('payload', JSON.stringify({ id: payload.id, ...payload.meta }));
    formData.append('file', payload.file);
    return formData;
  }

}