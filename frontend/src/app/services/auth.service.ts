import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { LoginRequest } from '../models/auth.models';
import { AuthResponse } from '../models/auth.models';
import { RegisterRequest } from '../models/auth.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiURL = 'https://localhost:7168/api/auth';

  constructor(private http: HttpClient, private router: Router) {}

  login(loginRequest: LoginRequest, rememberMe: boolean): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiURL}/login`, loginRequest).pipe(
      tap((res) => this.saveToken(res.token, rememberMe)),
      catchError(this.handleError.bind(this))
    );
  }

  register(registerRequest: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiURL}/register`, registerRequest).pipe(
      tap(() => {}),
      catchError(this.handleError.bind(this))
    );
  }

  logout(): void {
    localStorage.removeItem('access_token');
    sessionStorage.removeItem('access_token');
    this.router.navigate(['/home']);
  }

  getToken(): string | null {
    return sessionStorage.getItem('access_token') || localStorage.getItem('access_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Request failed';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Network error: ${error.error.message}`;
    } else if (error.status === 0) {
      errorMessage = 'Network error, please try again';
    } else {
      errorMessage = typeof error.error === 'string' ? error.error : error.error.message ? error.error.message : error.statusText || errorMessage;
    }
    return throwError(() => new Error(errorMessage));
  }

  private saveToken(token: string, rememberMe: boolean): void {
    if (rememberMe) {
      localStorage.setItem('access_token', token);
    } else {
      sessionStorage.setItem('access_token', token);
    }
  }
}
