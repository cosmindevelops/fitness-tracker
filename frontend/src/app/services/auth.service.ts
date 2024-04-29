import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { LoginRequest } from '../models/LoginRequest';
import { AuthResponse } from '../models/AuthResponse';
import { RegisterRequest } from '../models/RegisterRequest';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiURL = 'https://localhost:7168/api/auth';

  constructor(private http: HttpClient, private router: Router) { }

  login(loginRequest: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiURL}/login`, loginRequest)
      .pipe(
        tap(res => {
          localStorage.setItem('access_token', res.token); // Save token
          console.log('Login successful');
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Login attempt failed:', error);
          return throwError(() => new Error('Login attempt failed'));
        })
      );
  }

  register(registerRequest: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiURL}/register`, registerRequest)
      .pipe(
        tap(() => {
          console.log('Registration successful');
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Registration failed:', error);
          return throwError(() => new Error('Registration failed'));
        })
      );
  }

  logout(): void {
    localStorage.removeItem('access_token');
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('access_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
