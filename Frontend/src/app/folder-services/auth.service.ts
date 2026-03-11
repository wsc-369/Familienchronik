import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, tap } from 'rxjs';
import { ChangePasswordRequest, ForgotPasswordRequest, LoginRequest, LoginResponse } from './auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenStorageKey = 'auth.jwt';
  private readonly authBaseUrl = '/api/auth';
  private readonly isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());

  readonly isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private readonly http: HttpClient) {}

  login(payload: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.authBaseUrl}/login`, payload).pipe(
      tap((response) => {
        if (response?.accessToken) {
          this.setToken(response.accessToken);
        }
      })
    );
  }

  logout(): Observable<void> {
    this.clearToken();
    return of(void 0);
  }

  forgotPassword(payload: ForgotPasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.authBaseUrl}/forgot-password`, payload);
  }

  changePassword(payload: ChangePasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.authBaseUrl}/change-password`, payload);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenStorageKey);
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenStorageKey, token);
    this.isAuthenticatedSubject.next(true);
  }

  private clearToken(): void {
    localStorage.removeItem(this.tokenStorageKey);
    this.isAuthenticatedSubject.next(false);
  }

  private hasToken(): boolean {
    const token = localStorage.getItem(this.tokenStorageKey);
    return !!token && token.trim().length > 0;
  }
}
