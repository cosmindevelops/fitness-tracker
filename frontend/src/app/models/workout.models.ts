import { ExerciseResponseDto } from './exercise.models';

export interface WorkoutDto {
  notes: string;
  date: Date;
}

export interface WorkoutResponseDto {
  id: string;
  notes: string;
  date: Date;
  exercises: ExerciseResponseDto[];
}
