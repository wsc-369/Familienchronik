import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DialectWord } from '../api/api-interfaces';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class DialectWordService {

  constructor(private readonly http: HttpClient) {}

  getDialectWords(): Observable<DialectWord[]> {
    return this.http.get<DialectWord[]>(`${environment.apiUrl}/Dialect/getDialectWords`);
  }

 getDialectFilteredWords(word: string): Observable<DialectWord[]> {
  return this.http.get<DialectWord[]>(`${environment.apiUrl}/Dialect/getDialectFilteredWords/${word}`);
}
}
