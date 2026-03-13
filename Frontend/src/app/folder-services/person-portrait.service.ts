import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PersonPortrait } from '../api/api-interfaces';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PersonPortraitService {
  private readonly baseUrl = `${environment.apiUrl}/PersonPortrait`;

  constructor(private readonly http: HttpClient) {}

  getPersonPortraits(): Observable<PersonPortrait[]> {
    return this.http.get<PersonPortrait[]>(this.baseUrl);
  }

  getPersonPortraitById(id: string): Observable<PersonPortrait> {
    return this.http.get<PersonPortrait>(`${this.baseUrl}/${encodeURIComponent(id)}`);
  }

  createPersonPortrait(portrait: PersonPortrait): Observable<PersonPortrait> {
    return this.http.post<PersonPortrait>(this.baseUrl, portrait);
  }

  updatePersonPortrait(id: string, portrait: PersonPortrait): Observable<PersonPortrait> {
    return this.http.put<PersonPortrait>(`${this.baseUrl}/${encodeURIComponent(id)}`, portrait);
  }

  deletePersonPortrait(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${encodeURIComponent(id)}`);
  }
}
