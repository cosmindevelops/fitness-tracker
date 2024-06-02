import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginRequest } from '../models/auth.models';
import { AuthResponse } from '../models/auth.models';
import { RegisterRequest } from '../models/auth.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiURL = `${environment.apiUrl}/api/auth`;

  constructor(private http: HttpClient, private router: Router, private route: ActivatedRoute) {}

  login(loginRequest: LoginRequest, rememberMe: boolean): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiURL}/login`, loginRequest).pipe(
      tap((res) => this.saveToken(res.token, rememberMe)),
      catchError(this.handleError.bind(this))
    );
  }

  register(registerRequest: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiURL}/register`, registerRequest).pipe(
      catchError(this.handleError.bind(this)));
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

  loginWithGoogle(): void {
    window.location.href = `${this.apiURL}/google-login`;
  }

  handleGoogleLoginCallback(): void {
    this.route.queryParams.subscribe((params) => {
      const token = params['token'];
      if (token) {
        this.saveToken(token, true);
        this.router.navigate(['/workout']);
      }
    });
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
