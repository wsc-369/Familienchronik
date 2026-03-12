import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MediaLibraryDocument } from '../api/api-interfaces';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MediaLibraryDocumentService {

  constructor(private readonly http: HttpClient) {}

  getMediaLibraryDocuments(): Observable<MediaLibraryDocument[]> {
    return this.http.get<MediaLibraryDocument[]>(`${environment.apiUrl}/Document/SearchDocuments`);
  }

  getMediaLibraryFilteredDocuments(term: string): Observable<MediaLibraryDocument[]> {
    return this.http.get<MediaLibraryDocument[]>(`${environment.apiUrl}/Document/GetSearchSuggestions/${term}`);
  }
}
