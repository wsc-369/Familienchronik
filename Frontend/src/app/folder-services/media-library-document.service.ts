import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SearchResult } from '../api/api-interfaces';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MediaLibraryDocumentService {

  constructor(private readonly http: HttpClient) {}

  searchDocuments(term: string, pageIndex = 0, pageSize = 10): Observable<SearchResult> {
    const params = new HttpParams()
      .set('searchTerm', term)
      .set('pageIndex', pageIndex.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<SearchResult>(`${environment.apiUrl}/Document/SearchDocuments`, { params });
  }

  getSuggestions(term: string): Observable<string[]> {
    const params = new HttpParams().set('partialTerm', term);
    return this.http.get<string[]>(`${environment.apiUrl}/Document/GetSearchSuggestions`, { params });
  }
}

