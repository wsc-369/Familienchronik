import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContentTemplate } from '../api/api-interfaces';
import { environment } from '../../environments/environment';

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
}