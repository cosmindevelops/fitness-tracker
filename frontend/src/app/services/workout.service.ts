import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WorkoutDto, WorkoutResponseDto } from '../models/workout.models';

@Injectable({
  providedIn: 'root',
})
export class WorkoutService {
  private apiUrl = 'https://localhost:7168/api/workout';

  constructor(private http: HttpClient) {}

  getAllWorkouts(): Observable<WorkoutResponseDto[]> {
    return this.http.get<WorkoutResponseDto[]>(this.apiUrl);
  }

  getWorkoutById(workoutId: string): Observable<WorkoutResponseDto> {
    return this.http.get<WorkoutResponseDto>(`${this.apiUrl}/${workoutId}`);
  }

  createWorkout(workoutDto: WorkoutDto): Observable<WorkoutResponseDto> {
    return this.http.post<WorkoutResponseDto>(this.apiUrl, workoutDto);
  }

  updateWorkout(workoutId: string, workoutDto: WorkoutDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/${workoutId}`, workoutDto);
  }

  deleteWorkout(workoutId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${workoutId}`);
  }
}
