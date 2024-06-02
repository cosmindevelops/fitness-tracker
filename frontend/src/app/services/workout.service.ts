import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, catchError, tap, throwError } from 'rxjs';
import { WorkoutDto, WorkoutResponseDto } from '../models/workout.models';

@Injectable({
  providedIn: 'root',
})
export class WorkoutService {
  private apiUrl = 'http://localhost:80/api/workout';
  private workoutUpdated = new Subject<void>();

  constructor(private http: HttpClient) {}

  getAllWorkouts(): Observable<WorkoutResponseDto[]> {
    return this.http.get<WorkoutResponseDto[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }

  getWorkoutById(workoutId: string): Observable<WorkoutResponseDto> {
    return this.http.get<WorkoutResponseDto>(`${this.apiUrl}/${workoutId}`).pipe(
      catchError(this.handleError)
    );
  }

  getWorkouts(name?: string, date?: string): Observable<WorkoutResponseDto[]> {
    let params = new HttpParams();
    if (name) {
      params = params.set('name', name);
    }
    if (date) {
      params = params.set('date', date);
    }
    return this.http.get<WorkoutResponseDto[]>(`${this.apiUrl}/search`, { params }).pipe(
      catchError(this.handleError)
    );
  }

  createWorkout(workoutDto: WorkoutDto): Observable<WorkoutResponseDto> {
    return this.http.post<WorkoutResponseDto>(this.apiUrl, workoutDto).pipe(
      catchError(this.handleError),
      tap(() => {
        this.workoutUpdated.next();
      })
    );
  }

  updateWorkout(workoutId: string, workoutDto: WorkoutDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/${workoutId}`, workoutDto).pipe(
      catchError(this.handleError),
      tap(() => {
        this.workoutUpdated.next();
      })
    );
  }

  deleteWorkout(workoutId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${workoutId}`).pipe(
      catchError(this.handleError),
      tap(() => {
        this.workoutUpdated.next();
      })
    );
  }

  getWorkoutUpdateListener(): Observable<void> {
    return this.workoutUpdated.asObservable();
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An error occurred';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Network error: ${error.error.message}`;
    } else if (error.status === 0) {
      errorMessage = 'Network error, please try again';
    } else {
      errorMessage = error.error?.message || error.statusText || errorMessage;
    }
    return throwError(() => new Error(errorMessage));
  }
}
