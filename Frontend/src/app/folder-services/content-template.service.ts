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

  uploadContentTemplateImage(id: string, file: File): Observable<{ imageName: string }> {
    const formData = new FormData();
    formData.append('id', id);
    formData.append('file', file);
    return this.http.post<{ imageName: string }>(`${this.baseUrl}/uploadImage`, formData);
  }

  uploadContentTemplateDocument(id: string, file: File): Observable<UploadDocumentResponse> {
    const formData = new FormData();
    formData.append('id', id);
    formData.append('file', file);
    return this.http.post<UploadDocumentResponse>(`${this.baseUrl}/uploadDocument`, formData);
  }

}