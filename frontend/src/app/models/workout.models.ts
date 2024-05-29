import { ExerciseDto, ExerciseResponseDto } from './exercise.models';

export interface WorkoutDto {
  notes: string;
  date: Date;
  exercises?: ExerciseDto[];
}

export interface WorkoutResponseDto {
  id: string;
  notes: string;
  date: Date;
  exercises: ExerciseResponseDto[];
}
